"use client"

import { useState, useCallback, useEffect } from "react"
import { ChevronRight, ExternalLink, Home, Loader2 } from "lucide-react"
import { Switch } from "@/components/ui/switch"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Slider } from "@/components/ui/slider"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog"
import useAxiosPrivate from "@/hooks/useAxiosPrivate"
import useAuth from "@/hooks/useAuth"
import useUser from "@/hooks/useUser"
import { cn } from "@/lib/utils"
import { ProfileType } from "@/api/common"
import { Textarea } from "@/components/ui/textarea"
import updateUsername from "@/api/users/updateUsername"
import checkUsername from "@/api/users/checkUsername"
import getSettings, { SettingsResponse } from "@/api/users/getSettings"
import updateProfileType from "@/api/users/updateProfileType"
import { Label } from "@/components/ui/label"
import updateName from "@/api/users/updateName"
import updateBio from "@/api/users/updateBio"
import { useSearchParams } from "next/navigation"
import { getProfileImageSrc, getURL, MAX_USERNAME_LENGTH, USERNAME_REGEX } from "@/libs/helpers"
import ImageUploader from "@/components/ImageUploader"
import getAntiforgeryToken from "@/api/auth/getAntiforgeryToken"
import uploadProfileImage from "@/api/users/uploadProfileImage"
import deleteProfileImage from "@/api/users/deleteProfileImage"
import getSubscriptionPortal from "@/api/users/getSubscriptionPortal"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import useSWR from "swr"
import getOrCreateConnectedAccount from "@/api/users/getOrCreateConnectedAccount"
import updateExternalLink from "@/api/users/updateExternalLink"
import { toast } from "@/hooks/use-toast"

const MAX_BIO_LENGTH = 150

export default function SettingsContent() {
  const { axiosPrivate, isReady } = useAxiosPrivate();
  const { isAuthenticated, auth } = useAuth();
  const { user, setUser } = useUser();
  // const { data, isLoading } = useSWR(isReady && isAuthenticated && (user?.profileType === ProfileType.Artist || user?.profileType === ProfileType.Label) ? getOrCreateAlbumDraft.name : null, () => getOrCreateAlbumDraft(axiosPrivate, auth.userId!), {
  //   revalidateIfStale: true,
  //   revalidateOnFocus: false,
  //   revalidateOnReconnect: false
  // });
  const [username, setUsername] = useState(user?.username.toLowerCase() ?? "");
  const [name, setName] = useState(user?.name ?? "");
  const [settings, setSettings] = useState<SettingsResponse>();
  const [bio, setBio] = useState(settings?.bio ?? "");
  const [link, setLink] = useState(settings?.externalLink ?? "");
  const [isLinkValid, setIsLinkValid] = useState(true);
  const [isUsernameValid, setIsUsernameValid] = useState(true)
  const [isUsernameAvailable, setIsUsernameAvailable] = useState(false)
  const [isCheckingUsername, setIsCheckingUsername] = useState(false)
  const [isUpdating, setIsUpdating] = useState(false)
  const [autoplay, setAutoplay] = useState(true)
  const [profileType, setProfileType] = useState(user?.profileType ?? "")
  const [coverImageSrc, setCoverImageSrc] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [charsLeft, setCharsLeft] = useState(MAX_BIO_LENGTH - (settings?.bio ?? "").length);

  const fetchSettings = useCallback(async () => {
    if (isReady && isAuthenticated && auth.userId) {
      const settingsResult = await getSettings(axiosPrivate, auth.userId);
      if (settingsResult.ok) {
        setSettings(settingsResult.data);
        setBio(settingsResult.data?.bio ?? "");
        setLink(settingsResult.data?.externalLink ?? "");
      }
    }
  }, [isReady, isAuthenticated, axiosPrivate, auth.userId]);

  useEffect(() => {
    if (settings?.imageOriginalName && user?.profilePublicId) {
      setCoverImageSrc(getProfileImageSrc(user?.profilePublicId, true));
    }
  }, [user?.profilePublicId, settings?.imageOriginalName]);

  useEffect(() => {
    if (isReady && isAuthenticated) {
      fetchSettings();
    }
  }, [isReady, isAuthenticated, fetchSettings]);

  useEffect(() => {
    setCharsLeft(MAX_BIO_LENGTH - bio.length)
  }, [bio])

  useEffect(() => {
    setUsername(user?.username.toLowerCase() ?? "");
  }, [user?.username]);

  useEffect(() => {
    setName(user?.name ?? "");
  }, [user?.name]);

  useEffect(() => {
    setProfileType(user?.profileType ?? "");
  }, [user?.profileType]);

  const checkUsernameAvailability = useCallback(async (usernameValue: string) => {
    const isValid = USERNAME_REGEX.test(usernameValue);
    setIsUsernameValid(isValid)
    setIsUsernameAvailable(false);
    if (!isValid) {
      return;
    }
    setIsCheckingUsername(true);
    const checkResponse = await checkUsername(axiosPrivate, auth.userId!, usernameValue);
    setIsUsernameAvailable(checkResponse.ok)
    setIsCheckingUsername(false)
  }, [axiosPrivate, auth.userId])

  const updateUsernameHandler = useCallback(async () => {
    setIsCheckingUsername(true)
    const usernameToUpdate = username.toLowerCase();
    const updateResult = await updateUsername(axiosPrivate, auth.userId!, usernameToUpdate);
    if (updateResult.ok) {
      setUser(prev => ({ ...prev!, username: usernameToUpdate.toLowerCase() }));
      toast({ title: 'Username saved' });
    } else {
      setUsername(user?.username.toLowerCase() ?? "");
    }
    setIsCheckingUsername(false)
  }, [axiosPrivate, auth.userId, username, user?.username, setUser]);

  const updateNameHandler = useCallback(async () => {
    if (user?.name === name) {
      return;
    }

    setIsUpdating(true);
    const updateResult = await updateName(axiosPrivate, auth.userId!, name);
    if (updateResult.ok) {
      setUser(prev => ({ ...prev!, name: name }));
      toast({ title: 'Name saved' });
    } else {
      setName(user?.name ?? "");
    }
    setIsUpdating(false)
  }, [axiosPrivate, auth.userId, name, user?.name, setIsUpdating, setUser])

  const updateProfileTypeHandler = useCallback(async (value: string) => {
    setIsUpdating(true)
    const updateResult = await updateProfileType(axiosPrivate, auth.userId!, value);
    if (updateResult.ok) {
      setUser(prev => ({ ...prev!, profileType: value as ProfileType }));
      toast({ title: 'Profile type updated' });
    } else {
      setProfileType(user?.profileType ?? "");
    }
    setIsUpdating(false)
  }, [axiosPrivate, auth.userId, user?.profileType, setUser])

  const updateBioHandler = useCallback(async () => {
    setIsUpdating(true)
    const updateResult = await updateBio(axiosPrivate, auth.userId!, bio);
    if (updateResult.ok) {
      setSettings(prev => ({ ...prev!, bio: bio }));
      toast({ title: 'Bio saved' });
    } else {
      setBio(settings?.bio ?? "");
    }
    setIsUpdating(false)
  }, [axiosPrivate, auth.userId, settings?.bio, setSettings, setBio, bio, setIsUpdating])

  useEffect(() => {
    if (username.toLowerCase() == user?.username?.toLowerCase()) {
      return;
    }
    const timer = setTimeout(() => {
      checkUsernameAvailability(username)
    }, 700)
    return () => clearTimeout(timer)
  }, [username, user?.username, checkUsernameAvailability])

  const onUploadProfileImage = async (file: File, fileName: string) => {
    const tokenResponse = await getAntiforgeryToken(axiosPrivate);
    if (!tokenResponse.ok) {
      return false;
    }

    const response = await uploadProfileImage(axiosPrivate, auth.userId!, file, fileName, tokenResponse.data!);
    if (!response.ok) {
      return false;
    }

    setCoverImageSrc(getProfileImageSrc(user?.profilePublicId, true));
    toast({ title: 'Image saved' });

    return true;
  };

  const validateLink = (value: string) => {
    const urlRegex = /^(https:\/\/)/;
    setIsLinkValid(urlRegex.test(value));
  };

  const updateLinkHandler = useCallback(async () => {
    if (settings?.externalLink === link || !isLinkValid) {
      return;
    }
    setIsUpdating(true)
    const updateResult = await updateExternalLink(axiosPrivate, auth.userId!, link);
    if (updateResult.ok) {
      setSettings(prev => ({ ...prev!, link: link }));
      toast({ title: 'Link saved' });
    } else {
      setLink(settings?.externalLink ?? "");
    }
    setIsUpdating(false)
  }, [axiosPrivate, isLinkValid, auth.userId, settings?.externalLink, setSettings, setLink, link, setIsUpdating])

  const onDeleteProfileImage = async () => {
    const response = await deleteProfileImage(axiosPrivate, auth.userId!);
    if (response.ok) {
      setCoverImageSrc("");
      toast({ title: 'Image deleted' });
    }
  };

  const openPortal = useCallback(async () => {
    if (!isReady || !isAuthenticated) {
      return;
    }
    setIsLoading(true);
    const response = await getSubscriptionPortal(axiosPrivate, auth.userId!);
    setIsLoading(false);
    if (response.ok) {
      document.location.href = response.data!.url;
    }
  }, [isReady, axiosPrivate, isAuthenticated, auth.userId]);

  const getAccountLink = useCallback(async () => {
    if (!isReady || !isAuthenticated) {
      return;
    }

    setIsLoading(true);
    const response = await getOrCreateConnectedAccount(axiosPrivate, auth.userId!);
    setIsLoading(false);
    if (response.ok) {
      window.open(response.data!.url, '_blank');
    }
  }, [isReady, axiosPrivate, isAuthenticated, auth.userId, setIsLoading]);

  if (!isAuthenticated || !isReady || !user) {
    return (<></>);
  }

  const hasSubscription = user?.activeSubscriptions && user.activeSubscriptions.length > 0;
  const isArtistOrLabel = user?.profileType === ProfileType.Artist || user?.profileType === ProfileType.Label;

  return (
    <div className="min-h-screen bg-background p-4 sm:p-8">
      <div className="max-w-3xl mx-auto">
        <Tabs defaultValue="profile" className="w-full">
          <TabsList className={`grid w-full ${isArtistOrLabel ? "grid-cols-3" : "grid-cols-2"}`}>
            <TabsTrigger value="profile">Profile</TabsTrigger>
            <TabsTrigger value="subscriptions">Subscriptions</TabsTrigger>
            {isArtistOrLabel && <TabsTrigger value="royalties">Payouts</TabsTrigger>}
          </TabsList>
          <TabsContent value="profile" className="mt-6">
            <div className="space-y-4">
              <div className="flex flex-col space-y-2">
                <Label>Username</Label>
                <div className="space-y-2">
                  <div className="flex items-center space-x-2 relative">
                    <span className="absolute left-4 top-2.5 h-4 w-4 text-muted-foreground">@</span>
                    <Input
                      value={username}
                      disabled={isUpdating}
                      onChange={(e) => {
                        const newValue = e.target.value
                          .slice(0, MAX_USERNAME_LENGTH)
                          .replace(/[^a-zA-Z0-9.]/g, "");
                        setUsername(newValue);
                      }}
                      className="w-full !ml-0 px-8"
                      maxLength={MAX_USERNAME_LENGTH}
                    />
                    <Button disabled={isUpdating || !isUsernameValid || !isUsernameAvailable || (username.toLowerCase() === user.username.toLowerCase())} onClick={updateUsernameHandler}>Update</Button>
                  </div>
                  <div className={cn("text-sm", username.toLowerCase() === user.username.toLowerCase() ? "hidden" : "")}>
                    {isCheckingUsername ? (
                      <span className="text-muted-foreground">Checking availability...</span>
                    ) : !isUsernameValid ? (
                      <span className="text-red-600">Format is incorrect</span>
                    ) : isUsernameAvailable ? (
                      <span className="text-green-600">Username is available</span>
                    ) : (
                      <span className="text-red-600">Username is not available</span>
                    )}
                  </div>
                  <div className="text-xs text-muted-foreground">
                    Username must be 2-30 characters, contain only latin letters, numbers, dots, and cannot start or end with a dot or contain sequences of '..'
                  </div>
                </div>
              </div>
              <div className="flex flex-col space-y-2">
                <Label>Profile image</Label>
                <ImageUploader onUpload={onUploadProfileImage} uploadedImageUrl={coverImageSrc} onDelete={onDeleteProfileImage} cropShape="round" />
              </div>
              <div className="flex flex-col space-y-2">
                <Label>Name</Label>
                <div className="space-y-2">
                  <div className="flex items-center space-x-2 relative">
                    <Input
                      value={name}
                      disabled={isUpdating}
                      onChange={(e) => setName(e.target.value.slice(0, 50))}
                      onBlur={updateNameHandler}
                      className="w-full"
                      maxLength={50}
                    />
                  </div>
                </div>
              </div>
              <div className="flex flex-col space-y-2">
                <Label>Email</Label>
                <div className="space-y-2">
                  <div className="flex items-center space-x-2 relative">
                    <Input
                      value={auth.email}
                      disabled={true}
                      className="w-full"
                    />
                  </div>
                </div>
              </div>
              <div className="flex flex-col space-y-2">
                <Label>Profile Type</Label>
                <Select
                  value={user?.profileType ?? ""}
                  onValueChange={(v: string) => updateProfileTypeHandler(v)}
                  disabled={isUpdating || user.songCount > 0}
                >
                  <SelectTrigger className="w-full">
                    <SelectValue placeholder="Select" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value={ProfileType.Fan}>{ProfileType.Fan}</SelectItem>
                    <SelectItem value={ProfileType.Artist}>{ProfileType.Artist}</SelectItem>
                    <SelectItem value={ProfileType.Label}>{ProfileType.Label}</SelectItem>
                  </SelectContent>
                </Select>
                <div className="text-xs text-muted-foreground">
                  To upload music change the type to the Artist or Label. If you have released music the type can't be changed.
                </div>
              </div>
              <div className="flex flex-col space-y-2">
                <span className="text-foreground">Bio</span>
                <Textarea
                  disabled={isUpdating}
                  value={bio}
                  onChange={(e) => setBio(e.target.value.slice(0, MAX_BIO_LENGTH))}
                  placeholder="Type your bio here"
                  onBlur={updateBioHandler}
                  maxLength={150}
                  className="resize-none" />
                <div className="text-sm text-muted-foreground text-right">
                  {charsLeft} left
                </div>
              </div>
              <div className="flex flex-col space-y-2">
                <Label>Link</Label>
                <div className="space-y-2">
                  <div className="flex items-center space-x-2 relative">
                    <Input
                      value={link}
                      disabled={isUpdating}
                      onChange={(e) => {
                        const newValue = e.target.value;
                        setLink(newValue);
                        validateLink(newValue);
                      }}
                      onBlur={updateLinkHandler}
                      className="w-full"
                      placeholder="https://example.com"
                    />
                  </div>
                  {!isLinkValid && (
                    <div className="text-sm text-red-600">
                      Link must start with https://
                    </div>
                  )}
                  <div className="text-xs text-muted-foreground">
                    Add a link to your website or social media profile
                  </div>
                </div>
              </div>
            </div>
          </TabsContent>
          <TabsContent value="subscriptions" className="mt-6">
            <div className="space-y-6">
              <Card className="w-full">
                <CardHeader>
                  <CardTitle>Manage Your Subscription</CardTitle>
                  <CardDescription>{hasSubscription ? "View and manage your subscription details" : "No active subscriptions"}</CardDescription>
                </CardHeader>
                {hasSubscription && <CardContent>
                  <Button disabled={isLoading} className="w-full" size="lg" onClick={() => openPortal()}>
                    {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                    {!isLoading ? 'Go to Stripe Portal' : 'Redirecting to Stripe'}
                    <ExternalLink className="ml-2 h-4 w-4" />
                  </Button>
                </CardContent>}
              </Card>
            </div>
          </TabsContent>
          {isArtistOrLabel && <TabsContent value="royalties" className="mt-6">
            <div className="space-y-6">
              <Card className="w-full">
                <CardHeader>
                  <CardTitle>Manage Your Payouts</CardTitle>
                  <CardDescription>{isArtistOrLabel ? "View and manage your payout details" : "To "}</CardDescription>
                </CardHeader>
                <CardContent>
                  <Button disabled={isLoading} className="w-full" size="lg" onClick={() => getAccountLink()}>
                    {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                    {!isLoading ? 'Go to Stripe Portal' : 'Redirecting to Stripe'}
                    <ExternalLink className="ml-2 h-4 w-4" />
                  </Button>
                </CardContent>
              </Card>
            </div>
          </TabsContent>}
        </Tabs>
      </div>
    </div >
  )
}
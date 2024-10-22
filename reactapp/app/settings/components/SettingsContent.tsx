"use client"

import { useState, useCallback, useEffect } from "react"
import { ChevronRight, Home } from "lucide-react"
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

const usernameRegex = /^[a-zA-Z0-9._-]{2,30}$/
const MAX_USERNAME_LENGTH = 30
const MAX_BIO_LENGTH = 150

export default function SettingsContent() {
  const { axiosPrivate, isReady } = useAxiosPrivate();
  const { isAuthenticated, auth } = useAuth();
  const { user, setUser } = useUser();
  const [username, setUsername] = useState(user?.username ?? "");
  const [name, setName] = useState(user?.name ?? "");
  const [settings, setSettings] = useState<SettingsResponse>();
  const [bio, setBio] = useState(settings?.bio ?? "");
  const [isUsernameValid, setIsUsernameValid] = useState(true)
  const [isUsernameAvailable, setIsUsernameAvailable] = useState(false)
  const [isCheckingUsername, setIsCheckingUsername] = useState(false)
  const [isUpdating, setIsUpdating] = useState(false)
  const [isAlertOpen, setIsAlertOpen] = useState(false)
  const [autoplay, setAutoplay] = useState(true)
  const [profileType, setProfileType] = useState(user?.profileType ?? "")
  const [volume, setVolume] = useState(50)
  const [pauseWatchHistory, setPauseWatchHistory] = useState(false)
  const [usePreciseLocation, setUsePreciseLocation] = useState(true)

  const checkUsernameValidity = (username: string) => {
    return usernameRegex.test(username)
  }

  const fetchSettings = useCallback(async () => {
    if (isReady && isAuthenticated && auth.userId) {
      const settingsResult = await getSettings(axiosPrivate, auth.userId);
      if (settingsResult.ok) {
        setSettings(settingsResult.data);
      }
    }
  }, [isReady, isAuthenticated, axiosPrivate, auth.userId]);

  useEffect(() => {
    if (isReady && isAuthenticated) {
      fetchSettings();
    }
  }, [isReady, isAuthenticated, fetchSettings]);

  useEffect(() => {
    setUsername(user?.username ?? "");
  }, [user?.username]);

  useEffect(() => {
    setName(user?.name ?? "");
  }, [user?.name]);

  useEffect(() => {
    setProfileType(user?.profileType ?? "");
  }, [user?.profileType]);

  const checkUsernameAvailability = useCallback(async (username: string) => {
    const isValid = checkUsernameValidity(username)
    setIsUsernameValid(isValid)
    setIsUsernameAvailable(false);
    if (!isValid) {
      return;
    }
    setIsCheckingUsername(true);
    const checkResponse = await checkUsername(axiosPrivate, auth.userId!, username);
    setIsUsernameAvailable(checkResponse.ok)
    setIsCheckingUsername(false)
  }, [axiosPrivate, auth.userId])

  const updateUsernameHandler = useCallback(async () => {
    setIsCheckingUsername(true)
    const usernameToUpdate = username.toLowerCase();
    const updateResult = await updateUsername(axiosPrivate, auth.userId!, usernameToUpdate);
    if (updateResult.ok) {
      setUser(prev => ({ ...prev!, username: usernameToUpdate }));
    } else {
      setUsername(user?.username ?? "");
    }
    setIsCheckingUsername(false)
  }, [axiosPrivate, auth.userId, username, user?.username, setUser]);

  const updateNameHandler = useCallback(async () => {
    setIsUpdating(true);
    const updateResult = await updateName(axiosPrivate, auth.userId!, name);
    if (updateResult.ok) {
      setUser(prev => ({ ...prev!, name: name }));
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

  if (!isAuthenticated || !isReady || !user) {
    return (<></>);
  }


  return (
    <div className="min-h-screen bg-background p-4 sm:p-8">
      <div className="max-w-3xl mx-auto">
        <Tabs defaultValue="profile" className="w-full">
          <TabsList className="grid w-full grid-cols-3">
            <TabsTrigger value="profile">Profile</TabsTrigger>
            {/* <TabsTrigger value="playback">Playback</TabsTrigger>
            <TabsTrigger value="privacy">Privacy</TabsTrigger> */}
          </TabsList>
          <TabsContent value="profile" className="mt-6">
            <div className="space-y-6">
              <div className="flex flex-col space-y-2">
                <Label>Username</Label>
                <div className="space-y-2">
                  <div className="flex items-center space-x-2 relative">
                    <span className="absolute left-4 top-2.5 h-4 w-4 text-muted-foreground">@</span>
                    <Input
                      value={username}
                      disabled={isUpdating}
                      onChange={(e) => setUsername(e.target.value.slice(0, MAX_USERNAME_LENGTH))}
                      onKeyPress={(e) => {
                        const char = String.fromCharCode(e.which)
                        if (!char.match(/[a-zA-Z0-9._-]/)) {
                          e.preventDefault()
                        }
                      }}
                      className="w-full px-6"
                      maxLength={MAX_USERNAME_LENGTH}
                    />
                    <Button disabled={isUpdating || !isUsernameValid || !isUsernameAvailable || (username.toLowerCase() === user.username.toLowerCase())} onClick={updateUsernameHandler}>Update</Button>
                  </div>
                  <div className={cn("text-sm", username.toLowerCase() === user.username.toLowerCase() ? "hidden" : "")}>
                    {isCheckingUsername ? (
                      <span className="text-muted-foreground">Checking availability...</span>
                    ) : !isUsernameValid ? (
                      <span className="text-red-600">Invalid username format</span>
                    ) : isUsernameAvailable ? (
                      <span className="text-green-600">Username is available</span>
                    ) : (
                      <span className="text-red-600">Username is not available</span>
                    )}
                  </div>
                  <div className="text-xs text-muted-foreground">
                    Usernames can only use letters, numbers, underscores and periods.
                  </div>
                </div>
              </div>
              <div className="flex flex-col space-y-2">
                <Label>Name</Label>
                <div className="space-y-2">
                  <div className="flex items-center space-x-2 relative">
                    <Input
                      value={name}
                      disabled={isUpdating}
                      onChange={(e) => setName(e.target.value.slice(0, MAX_USERNAME_LENGTH))}
                      onBlur={updateNameHandler}
                      className="w-full"
                      maxLength={MAX_USERNAME_LENGTH}
                    />
                  </div>
                </div>
              </div>
              <div className="flex flex-col space-y-2">
                <Label>Profile Type</Label>
                <Select
                  value={user?.profileType ?? ""}
                  onValueChange={v => updateProfileTypeHandler(v)}
                  disabled={isUpdating}
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
                  To upload music change your profile type to the Artist or Label.
                </div>
              </div>
              <div className="flex flex-col space-y-2">
                <span className="text-foreground">Bio</span>
                <Textarea
                  disabled={isUpdating}
                  value={settings?.bio}
                  onChange={(e) => setBio(e.target.value.slice(0, MAX_BIO_LENGTH))}
                  placeholder="Type your bio here"
                  onBlur={updateBioHandler}
                  maxLength={150} />
              </div>
            </div>
          </TabsContent>
          <TabsContent value="playback" className="mt-6">
            <h2 className="text-2xl font-bold mb-4">Playback Settings</h2>
            <div className="space-y-6">
              <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between space-y-2 sm:space-y-0">
                <span className="text-foreground">Autoplay</span>
                <Switch
                  checked={autoplay}
                  onCheckedChange={setAutoplay}
                />
              </div>
              <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between space-y-2 sm:space-y-0">
                <span className="text-foreground">Audio quality</span>
                <Select
                  value={profileType}
                  onValueChange={setProfileType}
                >
                  <SelectTrigger className="w-full sm:w-[180px]">
                    <SelectValue placeholder="Select" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="low">Low</SelectItem>
                    <SelectItem value="medium">Medium</SelectItem>
                    <SelectItem value="high">High</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between space-y-2 sm:space-y-0">
                <span className="text-foreground">Default volume</span>
                <Slider
                  className="w-full sm:w-[180px]"
                  value={[volume]}
                  min={0}
                  max={100}
                  step={1}
                  onValueChange={([value]) => setVolume(value)}
                />
              </div>
            </div>
          </TabsContent>
          <TabsContent value="privacy" className="mt-6">
            <h2 className="text-2xl font-bold mb-4">Privacy Settings</h2>
            <div className="space-y-6">
              <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between space-y-2 sm:space-y-0">
                <span className="text-foreground">Pause watch history</span>
                <Switch
                  checked={pauseWatchHistory}
                  onCheckedChange={setPauseWatchHistory}
                />
              </div>
              <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between space-y-2 sm:space-y-0">
                <span className="text-foreground">Use precise location</span>
                <Switch
                  checked={usePreciseLocation}
                  onCheckedChange={setUsePreciseLocation}
                />
              </div>
            </div>
          </TabsContent>
        </Tabs>
      </div>
      <AlertDialog open={isAlertOpen} onOpenChange={setIsAlertOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. This will permanently change your username to {username}.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogAction onClick={() => {
              // Implement the actual save logic here
              setIsAlertOpen(false)
            }}>
              Continue
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
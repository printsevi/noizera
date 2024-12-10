'use client'

import { useState, useEffect } from 'react'
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Switch } from "@/components/ui/switch"
import { Label } from "@/components/ui/label"
import { useCookieConsent } from '@/providers/CookieConsentProvider'

type CookiePreferences = {
  necessary: boolean
  analytics: boolean
  marketing: boolean
}

export function CookieConsent() {
  const { consentGranted, setConsent } = useCookieConsent();
  const [hidden, setHidden] = useState<boolean>(true);
  const [preferences, setPreferences] = useState<CookiePreferences>({
    necessary: true,
    analytics: true,
    marketing: false,
  })

  useEffect(() => {
    if (!consentGranted) {
      setHidden(false);
    }
  }, [consentGranted]);

  const handleAccept = () => {
    setConsent(true, preferences.analytics);
    setHidden(true);
  }

  const handleReject = () => {
    setConsent(true, false);
    setHidden(true);
  }

  return (
    <Card className={`${hidden ? "hidden" : "block"} pb-safe fixed bottom-0 left-0 right-0 z-50 max-w-7xl mx-auto w-full border-t`}>
      <CardHeader>
        <CardTitle>Cookie Preferences</CardTitle>
      </CardHeader>
      <CardContent className="grid gap-4 py-4 sm:grid-cols-2 lg:grid-cols-3">
        <div className="flex items-center space-x-2">
          <Switch id="necessary" checked={preferences.necessary} disabled />
          <Label htmlFor="necessary" className="flex flex-col">
            <span className="font-medium">Necessary</span>
            <span className="text-sm text-muted-foreground">Required for the website to function properly</span>
          </Label>
        </div>
        <div className="flex items-center space-x-2">
          <Switch
            id="analytics"
            checked={preferences.analytics}
            onCheckedChange={(checked: any) => setPreferences(prev => ({ ...prev, analytics: checked }))}
          />
          <Label htmlFor="analytics" className="flex flex-col">
            <span className="font-medium">Analytics</span>
            <span className="text-sm text-muted-foreground">Help us improve our website</span>
          </Label>
        </div>
        {/* <div className="flex items-center space-x-2">
            <Switch
              id="marketing"
              checked={preferences.marketing}
              onCheckedChange={(checked) => setPreferences(prev => ({ ...prev, marketing: checked }))}
            />
            <Label htmlFor="marketing" className="flex flex-col">
              <span className="font-medium">Marketing</span>
              <span className="text-sm text-muted-foreground">Personalized ads and content</span>
            </Label>
          </div> */}
      </CardContent>
      <CardFooter className="flex flex-col sm:flex-row justify-end space-y-2 sm:space-y-0 sm:space-x-4 px-4 py-4">
        <Button variant="outline" onClick={handleReject} size="lg" className="w-full sm:w-[calc(50%-0.5rem)]">Reject All</Button>
        <Button onClick={handleAccept} size="lg" className="w-full sm:w-[calc(50%-0.5rem)]">Accept Selected</Button>
      </CardFooter>
    </Card>
  )
}


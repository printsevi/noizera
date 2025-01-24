'use client';

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Lock } from "lucide-react"
import PurpleButton from "./Button"
import useSignInModal from "@/hooks/useSignInModal"
import useSignUpModal from "@/hooks/useSignUpModal"

export default function AuthRequired() {
    const signInModal = useSignInModal();
    const signUpModal = useSignUpModal();

    return (
        <Card className="max-w-2xl mx-auto">
            <CardHeader className="text-center">
                <div className="mx-auto w-20 h-20 rounded-full flex items-center justify-center mb-4">
                    <Lock className="w-10 h-10" />
                </div>
                <CardTitle className="text-2xl md:text-3xl">Authentication Required</CardTitle>
            </CardHeader>
            <CardContent>
                <p className="text-center text-muted-foreground mb-6">
                    To view this page, you need to be logged in. Please log in to access the content.
                </p>
            </CardContent>
            <CardFooter className="flex flex-col space-y-4">
                <PurpleButton
                    onClick={signInModal.onOpen}
                    className='px-6 py-2'
                >
                    Log in
                </PurpleButton>
                <p className="text-sm text-center text-muted-foreground">
                    Don't have an account?{" "}
                    <Button onClick={signUpModal.onOpen} variant="link" className="underline">
                        Create free account
                    </Button>
                </p>
            </CardFooter>
        </Card>
    )
}
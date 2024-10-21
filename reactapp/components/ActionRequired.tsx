'use client';

import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Lock } from "lucide-react"
import PurpleButton from "./Button"
import { useRouter } from "next/navigation";

interface Props {
    description: string;
    buttonText: string;
    link: string;
}

export default function ActionRequired(props: Props) {
    const router = useRouter();

    return (
        <Card className="w-full">
            <CardHeader className="text-center">
                <div className="mx-auto w-20 h-20 rounded-full flex items-center justify-center mb-4">
                    <Lock className="w-10 h-10" />
                </div>
                <CardTitle className="text-2xl md:text-3xl">Action Required</CardTitle>
            </CardHeader>
            <CardContent>
                <p className="text-center text-muted-foreground mb-6">
                    {props.description}
                </p>
            </CardContent>
            <CardFooter className="flex flex-col space-y-4">
                <PurpleButton
                    onClick={() => router.push(props.link)}
                    className='bg-white px-6 py-2'
                >
                    {props.buttonText}
                </PurpleButton>
            </CardFooter>
        </Card>
    )
}
'use client';

import resetPassword from "@/api/auth/resetPassword";
import PasswordChecklist from "react-password-checklist"
import PurpleButton from "@/components/Button";
import { InputFormField } from "@/components/InputFormField";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Form } from "@/components/ui/form";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { toast } from "@/hooks/use-toast";
import useAuth from "@/hooks/useAuth";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import { PASSWORD_REGEX } from "@/libs/inputValidation";
import { zodResolver } from "@hookform/resolvers/zod";
import { CheckIcon, Loader2, MinusIcon, ReceiptEuro } from "lucide-react";
import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import useSWR from "swr";
import { z } from "zod";

const FormSchema = z.object({
  password: z.string().regex(PASSWORD_REGEX, {
    message: "Password is incorrect"
  })
});

interface Props {
  email: string,
  token: string
}

export default function ResetContent(props: Props) {
  const router = useRouter();

  const [isLoading, setIsLoading] = useState(false);
  const [inputPassword, setInputPassword] = useState("");

  const form = useForm<z.infer<typeof FormSchema>>({
    resolver: zodResolver(FormSchema),
    defaultValues: {
      password: "",
    },
  });

  const onPasswordChange = (value: string | undefined) => {
    if (value !== undefined) {
      setInputPassword(value);
    }
  };

  const onSubmit = async (data: z.infer<typeof FormSchema>) => {
    setIsLoading(true);

    if (!data.password) {
      toast({
        variant: "destructive",
        title: "Missing fields",
        description: "Please fill in missing fields",
      });
      setIsLoading(false);
      return;
    }

    if (!props.email || !props.token) {
      return;
    }

    const resetPasswordResponse = await resetPassword(props.email, props.token, data.password);
    if (!resetPasswordResponse.ok) {
      setIsLoading(false);
      return;
    }

    setIsLoading(false);
    toast({ title: "Your password is updated." });
    form.reset();
    setIsLoading(false);
    router.push('/');
  };

  if (!props.email || !props.token) {
    return (<></>);
  }

  return (<Card>
    <CardHeader className="text-center pb-2">
      <CardTitle className="!mb-7">Update your password</CardTitle>
    </CardHeader>
    <CardDescription className="text-center w-11/12 mx-auto">
      Fill in a new password
    </CardDescription>
    <CardContent>
      <Form {...form}>
        <form
          onSubmit={form.handleSubmit(onSubmit)}
          className='flex flex-col gap-y-4'
        >
          <InputFormField
            disabled={isLoading}
            name="password"
            label="Password"
            id="password"
            type="password"
            placeholder='type your password'
            onChange={onPasswordChange}
          />
          <PasswordChecklist
            rules={["minLength", "specialChar", "number", "capitalAndLowercase"]}
            iconSize={12}
            minLength={8}
            value={inputPassword}
            onChange={(isValid) => { }}
            className='text-sm'
          />
          <PurpleButton className='mt-auto' type="submit" disabled={isLoading}>
            {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {!isLoading ? 'Update password' : 'Updating password'}
          </PurpleButton>
        </form>
      </Form>
    </CardContent>
  </Card>);
}

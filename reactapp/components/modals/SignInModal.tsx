'use client';

import React, { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import useSignInModal from '@/hooks/useSignInModal';
import Modal from './Modal';
import { useForm } from 'react-hook-form';
import { zodResolver } from "@hookform/resolvers/zod"
import useAuth from '@/hooks/useAuth';
import PurpleButton from '../Button';
import useAuthenticationModal from '@/hooks/useAuthenticationModal';
import { InputFormField } from '../InputFormField';
import signIn from '@/api/auth/signIn';
import { z } from 'zod';
import { Form } from '../ui/form';
import { toast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import Link from 'next/link';
import { Button } from '../ui/button';
import useSignUpModal from '@/hooks/useSignUpModal';
import useForgotPasswordModal from '@/hooks/useForgotPasswordModal';

const FormSchema = z.object({
  emailOrUsername: z.string().min(1, {
    message: "Username or Email must be at least 1 character",
  }),
  password: z.string().min(8, {
    message: "Password must be at least 8 characters.",
  }),
});

const SignInModal = () => {
  const router = useRouter();
  const { onClose, isOpen } = useSignInModal();
  const authenticationModal = useAuthenticationModal();
  const signUpModal = useSignUpModal();
  const forgotPasswordModal = useForgotPasswordModal();
  const [isLoading, setIsLoading] = useState(false);

  const form = useForm<z.infer<typeof FormSchema>>({
    resolver: zodResolver(FormSchema),
    defaultValues: {
      emailOrUsername: "",
      password: ""
    },
  });

  const onChange = (open: boolean) => {
    if (!open) {
      onClose();
      form.reset();
    }
  };

  const onSignUpClick = () => {
    onClose();
    form.reset();
    signUpModal.onOpen();
  };

  const onSubmit = async (data: z.infer<typeof FormSchema>) => {
    setIsLoading(true);

    const emailOrUsername = data.emailOrUsername;
    const password = data.password;

    if (!emailOrUsername || !password) {
      toast({
        variant: "destructive",
        title: "Missing fields",
        description: "Please fill in missing fields",
      });
      setIsLoading(false);
      return;
    }

    const signInResponse = await signIn(emailOrUsername, password);
    if (!signInResponse.ok) {
      setIsLoading(false);
      return;
    }

    authenticationModal.setEmailOrUsername(emailOrUsername);

    setIsLoading(false);

    form.reset();
    onClose();
    authenticationModal.onOpen();
  };

  const onForgotPassword = async () => {
    onClose();
    form.reset();
    forgotPasswordModal.onOpen();
  };

  return (
    <Modal
      title='Sign In'
      description='Sign in to your account.'
      isOpen={isOpen}
      onChange={onChange}
    >
      <Form {...form}>
        <form
          onSubmit={form.handleSubmit(onSubmit)}
          className='flex flex-col gap-y-4'
        >
          <InputFormField
            disabled={isLoading}
            name="emailOrUsername"
            label="Email or Username"
            id="emailOrUsername"
            placeholder='type email or username'
          />
          <InputFormField
            disabled={isLoading}
            name="password"
            label="Password"
            id="password"
            type="password"
            placeholder='type your password'
          />
          <PurpleButton className='mt-auto' type="submit" disabled={isLoading}>
            {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {!isLoading ? 'Sign In' : 'Signing In'}
          </PurpleButton>
        </form>
      </Form>
      <div className="mt-4 text-center text-sm">
        <Button onClick={onForgotPassword} variant="link" className="underline">
          Forgot your password?
        </Button>
        <div>Don&apos;t have an account?
          <Button onClick={onSignUpClick} variant="link" className="underline">
            Sign up
          </Button>
        </div>
      </div>
    </Modal>
  );
};

export default SignInModal;

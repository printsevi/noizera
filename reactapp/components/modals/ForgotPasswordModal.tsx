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
import forgotPassword from '@/api/auth/forgotPassword';

const FormSchema = z.object({
  emailOrUsername: z.string().min(1, {
    message: "Username or Email must be at least 1 character",
  })
});

const ForgotPasswordModal = () => {
  const router = useRouter();
  const { onClose, isOpen } = useForgotPasswordModal();
  const [isLoading, setIsLoading] = useState(false);

  const form = useForm<z.infer<typeof FormSchema>>({
    resolver: zodResolver(FormSchema),
    defaultValues: {
      emailOrUsername: ""
    },
  });

  const onChange = (open: boolean) => {
    if (!open) {
      onClose();
      form.reset();
    }
  };

  const onSubmit = async (data: z.infer<typeof FormSchema>) => {
    setIsLoading(true);

    const emailOrUsername = data.emailOrUsername;

    if (!emailOrUsername) {
      toast({
        variant: "destructive",
        title: "Missing fields",
        description: "Please fill in missing fields",
      });
      setIsLoading(false);
      return;
    }

    const response = await forgotPassword(emailOrUsername);
    if (!response.ok) {
      setIsLoading(false);
      return;
    }

    setIsLoading(false);

    toast({ title: "Reset password link has been sent to your email" });

    form.reset();
    router.push('/')
    onClose();
  };

  return (
    <Modal
      title='Reset password'
      description='Reset your password'
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
          <PurpleButton className='mt-auto' type="submit" disabled={isLoading}>
            {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {!isLoading ? 'Reset' : 'Resetting'}
          </PurpleButton>
        </form>
      </Form>
    </Modal>
  );
};

export default ForgotPasswordModal;

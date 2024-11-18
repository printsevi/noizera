'use client';

import { useState } from 'react';
import { useForm } from 'react-hook-form';
import PasswordChecklist from "react-password-checklist"
import PurpleButton from '../Button';
import Modal from './Modal';
import useAuth from '@/hooks/useAuth';
import register from '@/api/auth/register';
import useRegistrationModal from '@/hooks/useRegistrationModal';
import useSignInModal from '@/hooks/useSignInModal';
import { PASSWORD_REGEX } from '@/libs/inputValidation';
import { InputFormField } from '../InputFormField';
import { Form } from '../ui/form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import { MAX_USERNAME_LENGTH, USERNAME_REGEX } from '@/libs/helpers';
import { sendGAEvent } from '@next/third-parties/google';

const FormSchema = z.object({
  username: z.string().min(2, {
    message: "Username must be at least 2 characters",
  })
    .max(MAX_USERNAME_LENGTH, `Username must not exceed ${MAX_USERNAME_LENGTH} characters.`)
    .regex(USERNAME_REGEX, {
      message: "Username can only contain letters, numbers, dots, and underscores, cannot start or end with a dot, and must not contain sequences of '..' or '__'."
    }),
  password: z.string().regex(PASSWORD_REGEX, {
    message: "Password is incorrect"
  })
});

const RegistrationModal = () => {
  const { onClose, isOpen, email } = useRegistrationModal();
  const signInModal = useSignInModal();
  const [isLoading, setIsLoading] = useState(false);
  const [inputPassword, setInputPassword] = useState("");

  const form = useForm<z.infer<typeof FormSchema>>({
    resolver: zodResolver(FormSchema),
    defaultValues: {
      username: "",
      password: "",
    },
  });

  const onChange = (open: boolean) => {
    if (!open) {
      onClose();
      form.reset();
    }
  };

  const onPasswordChange = (value: string | undefined) => {
    if (value !== undefined) {
      setInputPassword(value);
    }
  };

  const onSubmit = async (data: z.infer<typeof FormSchema>) => {
    setIsLoading(true);

    if (!data.username || !data.password) {
      toast({
        variant: "destructive",
        title: "Missing fields",
        description: "Please fill in missing fields",
      });
      setIsLoading(false);
      return;
    }

    const registerResponse = await register(email, data.username, data.password);
    if (!registerResponse) {
      setIsLoading(false);
      return;
    }

    sendGAEvent('button_click', {
      event_category: 'User Actions',
      event_label: 'Create Account Button Clicked',
      user_id: 'anonymous',
    });

    setIsLoading(false);
    onClose();
    toast({ title: "Your account has been created" });
    form.reset();
    signInModal.onOpen();
  };

  return (
    <Modal
      title='Create New Account'
      description='Almost done!'
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
            name="username"
            label="Username"
            id="username"
            placeholder='type username'
          />
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
            rules={["minLength", "specialChar", "number", "capital"]}
            iconSize={12}
            minLength={8}
            value={inputPassword}
            onChange={(isValid) => { }}
            className='text-sm'
          />
          <PurpleButton className='mt-auto' type="submit" disabled={isLoading}>
            {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {!isLoading ? 'Create account' : 'Creating account'}
          </PurpleButton>
        </form>
      </Form>
    </Modal>
  );
};

export default RegistrationModal;

'use client';

import signUp from '@/api/auth/signUp';
import useSignUpModal from '@/hooks/useSignUpModal';
import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import PurpleButton from '../Button';
import { InputFormField } from '../InputFormField';
import Modal from './Modal';
import useEmailVerificationModal from '@/hooks/useEmailVerificationModal';
import useAuth from '@/hooks/useAuth';
import { Loader2 } from 'lucide-react';
import Link from 'next/link';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from '@/hooks/use-toast';
import { Form } from '../ui/form';
import { Button } from '../ui/button';
import useSignInModal from '@/hooks/useSignInModal';

const FormSchema = z.object({
  email: z.string().email()
});

const SignUpModal = () => {
  const { onClose, isOpen } = useSignUpModal();
  const signInModal = useSignInModal();
  const emailVerificationModal = useEmailVerificationModal();
  const [isLoading, setIsLoading] = useState(false);

  const form = useForm<z.infer<typeof FormSchema>>({
    resolver: zodResolver(FormSchema),
    defaultValues: {
      email: ""
    },
  });

  const onChange = (open: boolean) => {
    if (!open) {
      onClose();
      form.reset();
    }
  };

  const onSignInClick = () => {
    onClose();
    form.reset();
    signInModal.onOpen();
  };

  const onSubmit = async (data: z.infer<typeof FormSchema>) => {
    setIsLoading(true);

    if (!data.email) {
      toast({
        variant: "destructive",
        title: "Missing fields",
        description: "Please fill in missing fields",
      });
      setIsLoading(false);
      return;
    }

    const signUpResponse = await signUp(data.email);
    if (!signUpResponse.ok) {
      setIsLoading(false);
      return;
    }

    emailVerificationModal.setEmail(data.email);

    setIsLoading(false);
    form.reset();
    onClose();
    emailVerificationModal.onOpen();
  };

  return (
    <Modal
      title='Create free account now'
      description='Enter your email below to create an account'
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
            name="email"
            label="Email"
            id="email"
            placeholder='type email'
          />
          <p className='text-center'>
            By clicking Sign Up, you agree to our <Link target='_blank' href="/terms" key="/terms" className="text-blue-500 hover:underline">Terms of Service and Privacy Policy</Link>
          </p>
          <div className="mt-4 text-center text-sm">
            <div>Already have an account?
              <Button onClick={onSignInClick} variant="link" className="underline">
                Log in
              </Button>
            </div>
          </div>
          <PurpleButton className='mt-auto' type="submit" disabled={isLoading}>
            {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {!isLoading ? 'Sign Up' : 'Signing Up'}
          </PurpleButton>
        </form>
      </Form>
    </Modal>
  );
};

export default SignUpModal;

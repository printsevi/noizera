'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import Modal from './Modal';
import useAuth from '@/hooks/useAuth';
import toast from 'react-hot-toast';
import useAuthenticationModal from '@/hooks/useAuthenticationModal';
import authenticate from '@/api/auth/authenticate';
import { decodeJwtToken } from '@/libs/helpers';
import { InputOTP, InputOTPGroup, InputOTPSlot } from '../ui/input-otp';
import { REGEXP_ONLY_DIGITS } from 'input-otp';

const AuthenticationModal = () => {
  const router = useRouter();
  const { onClose, isOpen, setEmailOrUsername, emailOrUsername } = useAuthenticationModal();
  const { signIn } = useAuth();
  const [isLoading, setIsLoading] = useState(false);
  const [succeed, setSucceed] = useState(false);
  const [otp, setOtp] = useState("");

  useEffect(() => {
    router.refresh();
    onClose();
  }, [succeed, router, onClose]);

  const onChange = (open: boolean) => {
    if (!open) {
      onClose();
    }
  };

  const onPinChange = async (pin: string) => {
    setOtp(pin);
    if (pin.length < 4) {
      return;
    }

    setIsLoading(true);

    const authenticateResponse = await authenticate(emailOrUsername, pin);
    if (!authenticateResponse){
      setIsLoading(false);
      return;
    }

    signIn(authenticateResponse.accessToken, authenticateResponse.refreshToken);
    
    setSucceed(true);
    setIsLoading(false);
    toast.success('Your email is verified');
    onClose();
    setEmailOrUsername("");
  }

  return (
    <Modal
      title='Email Verification'
      description='Enter the 4-digit verification code that was sent to your email'
      isOpen={isOpen}
      onChange={onChange}
    >
      <InputOTP 
        disabled={isLoading}
        maxLength={4} 
        pattern={REGEXP_ONLY_DIGITS} 
        onChange={onPinChange}>
        <InputOTPGroup>
          <InputOTPSlot index={0} />
          <InputOTPSlot index={1} />
          <InputOTPSlot index={2} />
          <InputOTPSlot index={3} />
        </InputOTPGroup>
      </InputOTP>
    </Modal>
  );
};

export default AuthenticationModal;

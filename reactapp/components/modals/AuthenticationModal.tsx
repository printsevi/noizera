'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import Modal from './Modal';
import useAuth from '@/hooks/useAuth';
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
    if (!authenticateResponse.ok) {
      setIsLoading(false);
      return;
    }

    signIn(authenticateResponse.data!.accessToken, authenticateResponse.data!.refreshToken);

    setSucceed(true);
    setIsLoading(false);
    onClose();
    setEmailOrUsername("");
    setOtp("");
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
        onChange={onPinChange}
        value={otp}>
        <InputOTPGroup>
          {Array.from({ length: 4 }).map((_, index) => (
            <InputOTPSlot
              key={index}
              index={index}
            />
          ))}
        </InputOTPGroup>
      </InputOTP>
    </Modal>
  );
};

export default AuthenticationModal;

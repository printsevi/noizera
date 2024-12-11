'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import Modal from './Modal';
import useAuth from '@/hooks/useAuth';
import useEmailVerificationModal from '@/hooks/useEmailVerificationModal';
import verifyEmail from '@/api/auth/verifyEmail';
import useRegistrationModal from '@/hooks/useRegistrationModal';
import { InputOTP, InputOTPGroup, InputOTPSlot } from '../ui/input-otp';
import { REGEXP_ONLY_DIGITS } from 'input-otp';
import { cn } from '@/lib/utils';

const EmailVerificationModal = () => {
  const router = useRouter();
  const { onClose, isOpen, setEmail, email } = useEmailVerificationModal();
  const registrationModal = useRegistrationModal();
  const [isLoading, setIsLoading] = useState(false);
  const [succeed, setSucceed] = useState(false);
  const [otp, setOtp] = useState("");

  useEffect(() => {
    router.refresh();
    onClose();
  }, [router, onClose, succeed]);

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

    const response = await verifyEmail(email, pin);
    if (!response.ok) {
      setIsLoading(false);
      return;
    }

    setIsLoading(false);
    setSucceed(true);
    setOtp("");
    onClose();
    setEmail("");
    registrationModal.setEmail(email);
    registrationModal.onOpen();
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

export default EmailVerificationModal;

'use client';

import { useEffect, useState } from 'react';
import SignInModal from '@/components/modals/SignInModal';
import AuthenticationModal from '@/components/modals/AuthenticationModal';
import SignUpModal from '@/components/modals/SignUpModal';
import EmailVerificationModal from '@/components/modals/EmailVerificationModal';
import RegistrationModal from '@/components/modals/RegistrationModal';
import UploadModal from '@/components/modals/UploadModal';

const ModalProvider: React.FC = () => {
  const [isMounted, setIsMounted] = useState(false);

  useEffect(() => {
    setIsMounted(true);
  }, []);

  if (!isMounted) {
    return null;
  }

  return (
    <>
      <SignInModal />
      <SignUpModal />
      <AuthenticationModal />
      <EmailVerificationModal />
      <RegistrationModal />
      <UploadModal />
    </>
  );
};

export default ModalProvider;

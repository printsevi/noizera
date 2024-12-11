'use client';

import signUp from '@/api/auth/signUp';
import useSignUpModal from '@/hooks/useSignUpModal';
import { email_validation } from '@/libs/inputValidation';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { FieldValues, FormProvider, SubmitHandler, useForm } from 'react-hook-form';
import PurpleButton from '../Button';
import { InputFormField } from '../InputFormField';
import Modal from './Modal';
import useEmailVerificationModal from '@/hooks/useEmailVerificationModal';
import useAuth from '@/hooks/useAuth';
import useUploadModal from '@/hooks/useUploadModal';
import { FileInput } from '../FileInput';

const UploadModal = () => {
  const router = useRouter();
  const { onClose, isOpen } = useUploadModal();
  const emailVerificationModal = useEmailVerificationModal();
  const [isLoading, setIsLoading] = useState(false);
  const [succeed, setSucceed] = useState(false);

  const methods = useForm();

  useEffect(() => {
    router.refresh();
    onClose();
  }, [router, onClose, succeed]);

  const onChange = (open: boolean) => {
    if (!open) {
      onClose();
    }
  };

  return (
    <Modal
      title='Upload a song file'
      description='Provide a WAV file'
      isOpen={isOpen}
      onChange={onChange}
    >
      {/* <FileInput/> */}
      <div></div>
    </Modal>
  );
};

export default UploadModal;

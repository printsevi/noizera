import { create } from 'zustand';

interface EmailVerificationModalStore {
  isOpen: boolean;
  onOpen: () => void;
  onClose: () => void;
  setEmail: (value: string) => void;
  email: string;
}

const useEmailVerificationModal = create<EmailVerificationModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({ isOpen: true }),
  onClose: () => set({ isOpen: false }),
  setEmail: (value: string) => set({ email: value }),
  email: ""
}));

export default useEmailVerificationModal;

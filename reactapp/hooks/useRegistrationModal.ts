import { create } from 'zustand';

interface RegistrationModalStore {
  isOpen: boolean;
  onOpen: () => void;
  onClose: () => void;
  setEmail: (value: string) => void,
  email: string;
}

const useRegistrationModal = create<RegistrationModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({ isOpen: true }),
  onClose: () => set({ isOpen: false }),
  setEmail: (value: string) => set({ email: value }),
  email: ""
}));

export default useRegistrationModal;

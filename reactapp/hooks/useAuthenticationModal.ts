import { create } from 'zustand';

interface AuthenticationModalStore {
  isOpen: boolean;
  onOpen: () => void;
  onClose: () => void;
  setEmailOrUsername: (value: string) => void,
  emailOrUsername: string;
}

const useAuthenticationModal = create<AuthenticationModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({ isOpen: true }),
  onClose: () => set({ isOpen: false }),
  setEmailOrUsername: (value: string) => set({ emailOrUsername: value }),
  emailOrUsername: ""
}));

export default useAuthenticationModal;

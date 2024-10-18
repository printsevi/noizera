import axios, { AxiosInstance } from 'axios';
import toast from 'react-hot-toast';

const signOut = async (axiosPrivate : AxiosInstance, userId: string): Promise<boolean | null> => {
  try {
    await axiosPrivate.post('/auth/sign-out',
      JSON.stringify({ userId: userId })
    );
    return true;
  } catch (err) {
    if (axios.isAxiosError(err)) {
        if (!err?.response) {
            toast.error('No server Response');
        } else if (err.response?.status === 400) {
            toast.error('Missing Email or Password');
        } else if (err.response?.status === 401) {
            toast.error('Unauthorized');
        } else {
            toast.error('Sign out Failed');
        }
    } else {
        toast.error('Something went wrong');
    }
  }

  return null;
};

export default signOut;

import { axiosPublic } from '@/libs/axios';
import axios from 'axios';
import toast from 'react-hot-toast';

const register = async (email: string, userName: string, password: string): Promise<boolean | null> => {
  try {
    await axiosPublic.post('/auth/register',
      JSON.stringify({ email: email, profileUserName: userName, password: password })
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
            toast.error('Sign Up Failed');
        }
    } else {
        toast.error('Something went wrong');
    }
  }

  return null;
};

export default register;

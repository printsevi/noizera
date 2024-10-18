import { axiosPublic } from '@/libs/axios';
import axios from 'axios';
import toast from 'react-hot-toast';

export interface AuthenticateResponse {
  accessToken: string;
  refreshToken: string;
}

const authenticate = async (emailOrUsername: string, code: string): Promise<AuthenticateResponse | null> => {
  try {
    const response = await axiosPublic.post<AuthenticateResponse>('/auth',
      JSON.stringify({ emailOrUsername: emailOrUsername, code: code })
    );
    return response.data;
  } catch (err) {
    if (axios.isAxiosError(err)) {
        if (!err?.response) {
            toast.error('No server Response');
        } else if (err.response?.status === 400) {
            toast.error('Missing Email or Password');
        } else if (err.response?.status === 401) {
            toast.error('Unauthorized');
        } else {
            toast.error('Sign in Failed');
        }
    } else {
        toast.error('Something went wrong');
    }
  }

  return null;
};

export default authenticate;

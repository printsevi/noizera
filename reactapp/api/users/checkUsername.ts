import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

const checkUsername = async (axiosPrivate : AxiosInstance, userId: string, username: string): Promise<ApiResponse<void>> => {
  const result : ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPrivate.get<void>(`/users/${userId}/check-username?username=${username}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
  }

  return result;
};

export default checkUsername;


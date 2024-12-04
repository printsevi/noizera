import { axiosPublic } from '@/libs/axios';
import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const submitContactForm = async (email: string, name: string, topic: string, description: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPublic.post<void>(`/users/contact`,
      JSON.stringify({ email: email, name: name, topic: topic, description: description })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default submitContactForm;

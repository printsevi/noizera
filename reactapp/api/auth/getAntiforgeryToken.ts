import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';

const getAntiforgeryToken = async (axiosPrivate : AxiosInstance): Promise<ApiResponse<string>> => {
  const result : ApiResponse<string> = { ok: true };
  try {
    const response = await axiosPrivate.get<string>(`/auth/antiforgery-token`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getAntiforgeryToken;


import { axiosPublic } from '@/libs/axios';
import axios from 'axios';
import { ApiResponse, handleErrorAndReturnProblem } from '../common';

export interface AuthenticateResponse {
  accessToken: string;
  refreshToken: string;
}

const authenticate = async (emailOrUsername: string, code: string): Promise<ApiResponse<AuthenticateResponse>> => {
  const result: ApiResponse<AuthenticateResponse> = { ok: true };
  try {
    const response = await axiosPublic.post<AuthenticateResponse>('/auth',
      JSON.stringify({ emailOrUsername: emailOrUsername, code: code })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default authenticate;

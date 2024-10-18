import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface SettingsResponse {
  name: string;
  bio?: string;
}

const getSettings = async (axiosPrivate : AxiosInstance, userId: string): Promise<ApiResponse<SettingsResponse>> => {
  const result : ApiResponse<SettingsResponse> = { ok: true };
  try {
    const response = await axiosPrivate.get<SettingsResponse>(`/users/${userId}/settings`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getSettings;


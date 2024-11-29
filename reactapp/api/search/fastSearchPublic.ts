// 'use server'

import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface FastSearchQueryResult {
  value: string
}

const fastSearchPublic = async (searchQuery: string): Promise<ApiResponse<FastSearchQueryResult[]>> => {
  const result: ApiResponse<FastSearchQueryResult[]> = { ok: true };
  try {
    const response = await axiosPublic.get<FastSearchQueryResult[]>(`/public/fast-search?searchQuery=${searchQuery}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default fastSearchPublic;


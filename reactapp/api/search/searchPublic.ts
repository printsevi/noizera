// 'use server'

import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface SearchQueryResult {
  title: string,
  publicId: string,
  imageId: string,
  category: string
}

const searchPublic = async (searchQuery: string): Promise<ApiResponse<SearchQueryResult[]>> => {
  const result: ApiResponse<SearchQueryResult[]> = { ok: true };
  try {
    const response = await axiosPublic.get<SearchQueryResult[]>(`/public/search?searchQuery=${searchQuery}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default searchPublic;


// 'use server'

import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';
import { GetFeedCategoriesResponse } from './getFeedCategories';

const getFeedPublicCategories = async ()
  : Promise<ApiResponse<GetFeedCategoriesResponse>> => {
  const result: ApiResponse<GetFeedCategoriesResponse> = { ok: true };
  try {
    const response = await axiosPublic.get<GetFeedCategoriesResponse>(`public/feed/categories`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getFeedPublicCategories;


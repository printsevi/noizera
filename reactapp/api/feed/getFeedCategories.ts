// 'use server'

import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';

export interface GetFeedCategoriesResponse {
  musicCategories: GetFeedMusicCollectionItem[];
  profileCategories: GetFeedProfileItem[];
}

export interface GetFeedMusicCollectionItem {
  title: string;
  api: string;
}

export interface GetFeedProfileItem {
  title: string;
  api: string;
}

const getFeedCategories = async (axiosPrivate : AxiosInstance, userId: string)
  : Promise<ApiResponse<GetFeedCategoriesResponse>> => {
  const result : ApiResponse<GetFeedCategoriesResponse> = { ok: true };
  try {
    const response = await axiosPrivate.get<GetFeedCategoriesResponse>(`/feed/categories?userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getFeedCategories;


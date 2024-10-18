import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';
import { axiosPublic } from '@/libs/axios';

export interface GetLatestTermsResponse {
  content: string;
  effectiveDate: string;
}

const getLatestTerms = async (): Promise<ApiResponse<GetLatestTermsResponse>> => {
  const result : ApiResponse<GetLatestTermsResponse> = { ok: true };
  try {
    const response = await axiosPublic.get<GetLatestTermsResponse>(`/users/latest-terms`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getLatestTerms;


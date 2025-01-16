import { ApiResponse, handleErrorAndReturnProblem, IdResponse } from '../common';
import { AxiosInstance } from 'axios';

const updateExternalLink = async (axiosPrivate: AxiosInstance, userId: string, link: string): Promise<ApiResponse<void>> => {
  const result: ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPrivate.put<void>(`/users/${userId}/external-link`,
      JSON.stringify({ externalLink: link })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default updateExternalLink;

import { ApiResponse, handleErrorAndReturnProblem, IdResponse, ProfileType } from '../common';
import { AxiosInstance } from 'axios';

const updateProfileType = async (axiosPrivate : AxiosInstance, userId: string, profileType: string): Promise<ApiResponse<void>> => {
  const result : ApiResponse<void> = { ok: true };
  try {
    const response = await axiosPrivate.put<void>(`/users/${userId}/profile-type`,
      JSON.stringify({ newProfileType: profileType })
    );
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default updateProfileType;

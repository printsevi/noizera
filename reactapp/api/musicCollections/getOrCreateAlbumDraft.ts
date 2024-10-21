// 'use server'

import { AxiosInstance } from 'axios';
import { ApiResponse, handleErrorAndReturnProblem, ProfileType } from '../common';

export interface GetOrCreateAlbumDraftResponse {
  albumId: string;
  albumPublicId: string;
  title: string;
  description?: string;
  coverImageMongoId?: string;
  coverImageOriginalName?: string;
  profileName: string;
  profileType: ProfileType;
  songs: GetOrCreateAlbumDraftSongResponse[];
  credits: GetOrCreateAlbumDraftCreditResponse[];
}

export interface GetOrCreateAlbumDraftSongResponse {
  key: string;
  title: string;
  songPublicId: string;
  originalFileName?: string;
  contentLength?: number;
  contentType?: string;
  sequence: number;
}

export interface GetOrCreateAlbumDraftCreditResponse {
  key: string;
  value: string;
}

const getOrCreateAlbumDraft = async (axiosPrivate: AxiosInstance, userId: string): Promise<ApiResponse<GetOrCreateAlbumDraftResponse>> => {
  const result: ApiResponse<GetOrCreateAlbumDraftResponse> = { ok: true };
  try {
    const response = await axiosPrivate.get<GetOrCreateAlbumDraftResponse>(`/music-collections/albums/draft?userId=${userId}`);
    result.data = response.data;
  } catch (err) {
    result.ok = false;
    result.problem = handleErrorAndReturnProblem(err);
  }

  return result;
};

export default getOrCreateAlbumDraft;


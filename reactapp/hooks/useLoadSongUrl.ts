//import { useSupabaseClient } from '@supabase/auth-helpers-react';

import { Song } from '@/types';

const useLoadSongUrl = (song: Song) => {
  // const supabaseClient = useSupabaseClient();

  // if (!song) {
  //   return '';
  // }

  // const { data: songData } = supabaseClient.storage
  //   .from('songs')
  //   .getPublicUrl(song.song_path);

  // return songData.publicUrl;

  return 'http://localhost:5173/api/songs/6af97a6c-907d-4d93-b552-4bb57578a8e2/stream';
};

export default useLoadSongUrl;

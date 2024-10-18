import SongContext from "@/providers/SongProvider";
import { useContext } from "react";

const useSong = () => {
    const context = useContext(SongContext);
    if (context === undefined) {
        throw new Error(`useAudio must be used within AudioPlayerProvider.`);
    }
    return context;
}

export default useSong;
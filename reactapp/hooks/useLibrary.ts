import LibraryContext from "@/providers/LibraryProvider";
import { useContext } from "react";

const useLibrary = () => {
    const context = useContext(LibraryContext);
    if (context === undefined) {
        throw new Error(`useLibrary must be used within LibraryProvider.`);
    }
    return context;
}

export default useLibrary;
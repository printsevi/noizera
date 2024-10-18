import AuthContext from "@/providers/AuthProvider";
import { useContext } from "react";

const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error(`useUser must be used within a AuthProvider.`);
    }
    return context;
}

export default useAuth;
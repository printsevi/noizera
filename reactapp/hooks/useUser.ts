import UserContext from "@/providers/UserProvider";
import { useContext } from "react";

const useUser = () => {
    const context = useContext(UserContext);
    if (context === undefined) {
        throw new Error(`useMyUser must be used within a UserProvider.`);
    }
    return context;
}

export default useUser;
import { useEffect, useState } from 'react';

export function useIsIOS() {
    const [isIOS, setIsIOS] = useState(false);

    useEffect(() => {
        const userAgent = window.navigator.userAgent.toLowerCase();
        const iosDevice = /iphone|ipad|ipod/.test(userAgent);
        setIsIOS(iosDevice);
    }, []);

    return isIOS;
}


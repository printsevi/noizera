'use client';

import useLocalStorage from "@/hooks/useLocalStorage";
import { disableTracking, enableTracking } from "@/libs/helpers";
import { createContext, ReactNode, useContext, useEffect, useState } from "react";

type CookieConsentContextType = {
    consentGranted: boolean;
    setConsent: (value: boolean, analytics: boolean) => void;
};

const CookieConsentContext = createContext<CookieConsentContextType | undefined>(undefined);

export const CookieConsentProvider = ({ children }: { children: ReactNode }) => {
    const [consentGranted, setConsentGranted] = useLocalStorage<boolean>("consentGranted", false);
    const [analyticsGranted, setAnalyticsGranted] = useLocalStorage<boolean>("analyticsGranted", false);
    useEffect(() => {
        if (consentGranted && analyticsGranted) {
            enableTracking();
        }
    }, [consentGranted]);

    const setConsent = (value: boolean, analytics: boolean) => {
        setConsentGranted(value);
        setAnalyticsGranted(analytics);
        if (analytics) {
            enableTracking(); // Enable tracking globally
        } else {
            disableTracking(); // Disable tracking globally
        }
    };

    return (
        <CookieConsentContext.Provider value={{ consentGranted, setConsent }}>
            {children}
        </CookieConsentContext.Provider>
    );
};

export const useCookieConsent = () => {
    const context = useContext(CookieConsentContext);
    if (!context) {
        throw new Error("useCookieConsent must be used within a CookieConsentProvider");
    }
    return context;
};

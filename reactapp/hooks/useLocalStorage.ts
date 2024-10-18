import { useState, useEffect } from "react";

const getStorageValue = <T>(key: string, defaultValue: T): T => {
    try {
        const saved = window.localStorage.getItem(key);
        return saved ? JSON.parse(saved) : defaultValue;
    } catch(e){}
    return defaultValue;
};

const useLocalStorage = <T>(
    key: string,
    defaultValue: T,
): [T, (newValue: T) => void] => {
  const [value, setValue] = useState<T>(() =>
    getStorageValue(key, defaultValue),
  );

  useEffect(() => {
    try {
        localStorage.setItem(key, JSON.stringify(value));
    } catch(e){}
  }, [key, value]);

  return [value, setValue];
};

export default useLocalStorage;
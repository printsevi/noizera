import axios from 'axios';
import { getURL } from './helpers';

const BASE_URL = `${getURL()}api`;

export const axiosPublic = axios.create({
    baseURL: BASE_URL,
    headers: { 'Content-Type': 'application/json' }
});

export const axiosPrivate = axios.create({
    baseURL: BASE_URL,
    headers: { 'Content-Type': 'application/json' },
    withCredentials: true
});
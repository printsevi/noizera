import { DecodedToken, Price, UserDetails } from '@/types';
import { jwtDecode } from "jwt-decode";

export const getURL = () => {
  let url =
    process?.env?.NEXT_PUBLIC_SUPABASE_URL ??
    'https://noizera.com';
  // Make sure to include `https://` when not localhost.
  url = url.includes('http') ? url : `https://${url}`;
  // Make sure to including trailing `/`.
  url = url.charAt(url.length - 1) === '/' ? url : `${url}/`;
  return url;
};

export const postData = async ({
  url,
  data,
}: {
  url: string;
  data?: { price: Price };
}) => {
  console.log('posting,', url, data);

  const res: Response = await fetch(url, {
    method: 'POST',
    headers: new Headers({ 'Content-Type': 'application/json' }),
    credentials: 'same-origin',
    body: JSON.stringify(data),
  });

  if (!res.ok) {
    console.log('Error in postData', { url, data, res });

    throw Error(res.statusText);
  }

  return res.json();
};

export const toDateTime = (secs: number) => {
  var t = new Date('1970-01-01T00:30:00Z'); // Unix epoch start.
  t.setSeconds(secs);
  return t;
};

export const decodeJwtToken = (jwtToken: string) => {
  let decodedToken = jwtDecode<DecodedToken>(jwtToken);
  return decodedToken;
};

export default function getCroppedImg(
  imageSrc: string,
  croppedAreaPixels: { x: number; y: number; width: number; height: number },
  fileType: 'image/jpeg' | 'image/png' = 'image/jpeg'
): Promise<File> {
  return new Promise((resolve, reject) => {
    const image = new Image();
    image.src = imageSrc;
    image.onload = () => {
      const canvas = document.createElement('canvas');
      const ctx = canvas.getContext('2d');

      if (!ctx) {
        reject(new Error('Could not get canvas context'));
        return;
      }

      canvas.width = croppedAreaPixels.width;
      canvas.height = croppedAreaPixels.height;

      ctx.drawImage(
        image,
        croppedAreaPixels.x,
        croppedAreaPixels.y,
        croppedAreaPixels.width,
        croppedAreaPixels.height,
        0,
        0,
        croppedAreaPixels.width,
        croppedAreaPixels.height
      );

      canvas.toBlob(
        (blob) => {
          if (blob) {
            // Convert Blob to File
            const file = new File([blob], 'cropped_image.jpeg', { type: fileType });
            resolve(file);
          } else {
            reject(new Error('Canvas is empty'));
          }
        },
        fileType,
        0.9 // Adjust compression quality if needed
      );
    };
    image.onerror = (error) => reject(error);
  });
}





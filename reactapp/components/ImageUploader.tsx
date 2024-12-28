import React, { useState, useCallback, useEffect, useRef } from 'react';
import Cropper from 'react-easy-crop';
import imageCompression from 'browser-image-compression';
import axios from 'axios';
import Image from "next/image"
import getCroppedImg from '@/libs/helpers';
import getAntiforgeryToken from '@/api/auth/getAntiforgeryToken';
import useAxiosPrivate from '@/hooks/useAxiosPrivate';
import useAuth from '@/hooks/useAuth';
import uploadAlbumCoverImage from '@/api/musicCollections/uploadAlbumCoverImage';
import { AspectRatio } from './ui/aspect-ratio';
import { Label } from './ui/label';
import { Input } from './ui/input';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Button } from './ui/button';
import { Slider } from './ui/slider';
import { FileImage, Trash2, Upload } from 'lucide-react';

interface ImageUploadProps {
  uploadedImageUrl?: string;
  onUpload: (file: File, fileName: string) => Promise<boolean>;
  onDelete: () => Promise<void>;
  cropShape?: 'rect' | 'round';
}

const ImageUploader: React.FC<ImageUploadProps> = ({ uploadedImageUrl, onUpload, onDelete, cropShape = 'rect' }) => {
  const [imageSrc, setImageSrc] = useState<string | undefined>(undefined);
  const [isCropperOpen, setIsCropperOpen] = useState(false);
  const [fileName, setFileName] = useState<string>('');
  const [croppedArea, setCroppedArea] = useState<any>(null);
  const [crop, setCrop] = useState({ x: 0, y: 0 });
  const [zoom, setZoom] = useState(1);
  const [uploading, setUploading] = useState(false);

  useEffect(() => {
    if (imageSrc) {
      setIsCropperOpen(true);
    }
  }, [imageSrc]);

  const onCropComplete = useCallback((_: any, croppedAreaPixels: any) => {
    setCroppedArea(croppedAreaPixels);
  }, []);

  const onCropperChange = (open: boolean) => {
    setIsCropperOpen(open);
  };

  const handleZoomChange = useCallback((newZoom: number) => {
    setZoom(newZoom);
  }, []);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        setImageSrc(reader.result as string);
      };
      reader.readAsDataURL(file);
      setFileName(file.name);
    }
  };

  const handleCrop = useCallback(async () => {
    try {
      if (imageSrc && croppedArea) {
        // Get the cropped image as a File object
        const croppedFile = await getCroppedImg(imageSrc, croppedArea, 'image/jpeg');

        // Compress the image
        const options = {
          maxSizeMB: 1,
          maxWidthOrHeight: 544,
          useWebWorker: true,
        };
        const compressedFile = await imageCompression(croppedFile, options);

        setUploading(true);

        await onUpload(compressedFile, fileName);
        setImageSrc(undefined);
        setIsCropperOpen(false);
        setZoom(1);
      }
    } catch (error) {
      console.error('Error uploading the image:', error);
    } finally {
      setUploading(false);
    }
  }, [croppedArea, imageSrc]);

  return (
    <div className="flex flex-col w-full max-w-xs">
      <AspectRatio ratio={1}>
        {!uploadedImageUrl ? (
          <div className={`relative flex items-center justify-center w-full h-full bg-muted ${cropShape === 'round' ? 'rounded-full' : 'rounded-md'} overflow-hidden`}>
            <Input
              id="image-file"
              type="file"
              accept="image/*"
              onChange={handleFileChange}
              className="sr-only"
            />
            <Label
              htmlFor="image-file"
              className="flex flex-col items-center justify-center w-full h-full cursor-pointer transition-colors hover:bg-muted-foreground/10"
            >
              <Upload size={48} className="mb-2 text-muted-foreground" />
              <span className="text-sm font-medium text-muted-foreground">Upload image</span>
            </Label>
          </div>
        ) : (
          <div className={`group relative w-full h-full ${cropShape === 'round' ? 'rounded-full' : 'rounded-md'} overflow-hidden`}>
            <Image
              src={uploadedImageUrl}
              alt="Uploaded cover image"
              fill
              className="object-cover"
            />
            <div className="absolute inset-0 flex items-center justify-center bg-black bg-opacity-50 opacity-0 group-hover:opacity-100 transition-opacity">
              <Button
                type="button"
                variant="ghost"
                size="icon"
                onClick={onDelete}
                className="rounded-full hover:text-red-500 transition-colors"
              >
                <Trash2 size={24} />
                <span className="sr-only">Delete image</span>
              </Button>
            </div>
          </div>
        )}
      </AspectRatio>
      <Dialog open={isCropperOpen} defaultOpen={isCropperOpen} onOpenChange={onCropperChange}>
        <DialogContent className="max-w-[425px]">
          <DialogHeader>
            <DialogTitle>Crop image</DialogTitle>
            <DialogDescription>
              Make changes to your image here. Click Upload when you're done.
            </DialogDescription>
          </DialogHeader>
          <div className="relative w-full h-64">
            <Cropper
              cropShape={cropShape}
              image={imageSrc}
              crop={crop}
              zoom={zoom}
              aspect={1}
              onCropChange={setCrop}
              onZoomChange={handleZoomChange}
              onCropComplete={onCropComplete}
              minZoom={1}
              maxZoom={5}
            />
            <Slider
              defaultValue={[1]}
              min={1}
              max={5}
              step={0.1}
              value={[zoom]}
              onValueChange={(e) => handleZoomChange(e[0])}
            />
          </div>
          <DialogFooter>
            <Button onClick={handleCrop} disabled={uploading}>{uploading ? 'Uploading...' : 'Crop & Upload'}</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default ImageUploader;

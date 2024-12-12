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
import { FileImage, Trash2 } from 'lucide-react';

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
    <div className="flex flex-col w-56">
      {!uploadedImageUrl ? (
        <div className="relative flex-1 w-full">
          <Input
            id="image-file"
            type="file"
            accept="image/*"
            onChange={handleFileChange}
            className="sr-only"
          />
          <label
            htmlFor="image-file"
            className="flex items-center justify-between w-full py-2 h-10 px-3 text-sm border rounded-md cursor-pointer transition-colors ring-offset-background hover:outline-none hover:ring-2 hover:ring-ring hover:ring-offset-2"
          >
            <span>Choose file</span>
            <FileImage size={20} />
          </label>
        </div>)
        : (
          <div>
            <div className={`bg-muted ${cropShape === "round" ? "rounded-full overflow-hidden" : ""}`}>
              <Image
                src={uploadedImageUrl ?? ""}
                width={500}
                height={500}
                alt="Uploaded cover image"
                className="rounded-md object-cover max-w-full h-auto block"
              />
            </div>
            <div className="border rounded-md mb-1 py-2 px-3 rounded-md flex flex-row items-start sm:items-center justify-between space-y-2 sm:space-y-0">
              <div className="flex items-center space-x-2">
                <div>
                  <p className="font-medium">Cover image</p>
                  <p className="text-sm text-muted-foreground">Uploaded</p>
                </div>
              </div>
              <Button
                type="button"
                variant="ghost"
                size="icon"
                onClick={onDelete}
                className="rounded-full"
              >
                <Trash2 size={20} />
                <span className="sr-only">Delete file</span>
              </Button>
            </div>
          </div>
        )}
      <Dialog open={isCropperOpen} defaultOpen={isCropperOpen} onOpenChange={onCropperChange}>
        <DialogContent className="sm:max-w-[425px]">
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
              minZoom={0.5}
              maxZoom={5}
            />
            <Slider
              defaultValue={[1]}
              min={1}
              max={3}
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

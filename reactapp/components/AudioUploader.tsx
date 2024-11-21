import React, { useState } from 'react';
import { Button } from './ui/button';
import { Label } from './ui/label';
import { Input } from './ui/input';
import { FileAudio, Loader2, Trash2, Upload } from 'lucide-react';

interface AudioUploaderProps {
  existingFileName: string;
  uploadTime: string;
  audioSrc: string;
  contentType?: string;
  onFileUpload: (file: File) => Promise<boolean>;
  onFileDelete: () => Promise<void>;
}

const AudioUploader: React.FC<AudioUploaderProps> = ({ contentType, existingFileName: uploadedFileName, uploadTime, audioSrc, onFileUpload, onFileDelete }) => {
  const [file, setFile] = useState<File | null>(null);
  const [error, setError] = useState<string>('');
  const [isUploading, setIsUploading] = useState<boolean>(false);

  const MAX_FILE_SIZE = 300 * 1024 * 1024; // 300MB

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = e.target.files?.[0];

    if (selectedFile) {
      if (selectedFile.size > MAX_FILE_SIZE) {
        setError('File size exceeds the 300MB limit.');
        setFile(null);
      } else if (!selectedFile.type.startsWith('audio/wav')
        && !selectedFile.type.startsWith('audio/aif')
        && !selectedFile.type.startsWith('audio/aiff')
      ) {
        setError('Please upload .wav or .aif/aiff');
        setFile(null);
      } else {
        setError('');
        setFile(selectedFile);
        setIsUploading(true);
        const response = await onFileUpload(selectedFile);
        setFile(null);
        setIsUploading(false);
      }
    }
  };

  return (
    <div>
      <div className="flex flex-col mb-2 sm:flex-row items-center space-y-2 sm:space-y-0 sm:space-x-2">
        {!uploadedFileName && (<div className="relative flex-1 w-full">
          <Input
            id="audio-file"
            type="file"
            accept="audio/wav,.aif,audio/aiff"
            onChange={handleFileChange}
            className="sr-only"
            disabled={isUploading}
          />
          <label
            htmlFor="audio-file"
            className="flex items-center justify-between w-full py-2 h-10 px-3 text-sm border rounded-md cursor-pointer transition-colors ring-offset-background hover:outline-none hover:ring-2 hover:ring-ring hover:ring-offset-2"
          >
            <span>
              {file ? file.name : "Choose file"}
            </span>
            {isUploading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {!isUploading && <FileAudio size={20} />}
          </label>
        </div>)}
      </div>
      {error && <p className="text-red-500 mt-2">{error}</p>}

      {uploadedFileName && (
        <div className="border rounded-md mb-1 py-2 px-3 rounded-md flex flex-row items-start sm:items-center justify-between space-y-2 sm:space-y-0">
          <div className="flex items-center space-x-2">
            <div>
              <p className="font-medium">{uploadedFileName}</p>
              <p className="text-sm text-muted-foreground">Uploaded: {uploadTime}</p>
              {contentType === 'audio/wav' && <div>
                <audio controls>
                  <source src={audioSrc} type={contentType} />
                  Your browser does not support the audio tag.
                </audio>
              </div>}
            </div>
          </div>
          <Button
            type="button"
            variant="ghost"
            size="icon"
            onClick={onFileDelete}
            className="rounded-full"
          >
            <Trash2 size={20} />
            <span className="sr-only">Delete file</span>
          </Button>
        </div>
      )}
    </div>
  );
};

export default AudioUploader;

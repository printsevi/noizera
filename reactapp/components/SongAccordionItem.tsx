import React, { useState } from 'react';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { RxDragHandleHorizontal } from "react-icons/rx";
import { FaChevronDown } from 'react-icons/fa';
import { InputField } from './InputField';
import { FileInput } from './FileInput';
import { getURL } from '@/libs/helpers';
import useUploadModal from '@/hooks/useUploadModal';
import { Button } from './ui/button';
import { ChevronDown, Delete, EllipsisVertical, Grip, GripHorizontal, MoreHorizontal, Trash2 } from 'lucide-react';
import { FileUploadDialog } from './FileUploadDialog';
import { Label } from './ui/label';
import { Input } from './ui/input';
import AudioUploader from './AudioUploader';
import deleteAudioFile from '@/api/songs/deleteAudioFile';
import useUser from '@/hooks/useUser';

interface Props {
    id: string,
    key: string,
    isOpen: boolean,
    title?: string,
    audioFileName?: string;
    songPublicId: string;
    toggleAccordion: () => void,
    onTitleUpdate: (newTitle: string) => Promise<boolean>;
    onAudioUpload: (file: File) => Promise<string | undefined>;
    onSongDelete: () => Promise<void>;
    onAudioDelete: () => Promise<void>;
}
export const SongAccordionItem = (props: Props) => {
    const {
        attributes,
        listeners,
        setNodeRef,
        transform,
        transition,
    } = useSortable({ id: props.id });

    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
    };

    const [songTitle, setSongTitle] = useState(props.title ?? "");

    const [audioFileName, setAudioFileName] = useState(props.audioFileName);

    const { user } = useUser();

    const updateTitle = async (newTitle: string) => {
        const result = await props.onTitleUpdate(newTitle);
        if (!result) {
            setSongTitle(props.title ?? "");
        }
    }

    const uploadAudio = async (file: File) => {
        const result = await props.onAudioUpload(file);
        if (result !== undefined) {
            setAudioFileName(result);
            return true;
        }
        return false;
    }

    const deleteAudioHandler = async () => {
        const response = await props.onAudioDelete();
    }

    const audioSrc = audioFileName ? `${getURL()}api/songs/${props.songPublicId}/audio` : "";

    return (
        <div
            ref={setNodeRef}
            style={style}
            className="border rounded-md mb-1 ring-offset-background hover:outline-none hover:ring-2 hover:ring-ring hover:ring-offset-2"
        >
            <button
                {...attributes}
                {...listeners}
                className="w-full p-2 text-left transition duration-300"
                onClick={props.toggleAccordion}
            >
                <span className='flex flex-row h-auto items-center w-full gap-x-4 justify-between'>
                    <span className='flex'>
                        <span className={`transform ${props.isOpen ?
                            'rotate-180' : 'rotate-0'}  
                                        transition-transform duration-300`}>
                            <ChevronDown size={20} />
                        </span>
                        <span className='truncate'>{user?.name ?? user?.username} - {songTitle ? songTitle : `ID`}</span>
                    </span>


                    <span className='flex items-center'>
                        <Grip size={20} />
                        <Button variant='ghost' className="rounded-full" size="icon"
                            onClick={(e) => {
                                e.stopPropagation();
                                props.onSongDelete();
                            }} >
                            <Trash2 size={20} />
                        </Button>
                    </span>


                </span>
            </button>
            {props.isOpen && (
                <div className="p-4">
                    <Label>Song title</Label>
                    <Input
                        value={songTitle}
                        onBlur={(value) => updateTitle(value.target.value)}
                        onChange={(value) => setSongTitle(value.target.value)}
                        placeholder='type your song title'
                    />
                    <Label>Audio</Label>
                    <AudioUploader uploadTime='00/00/0000' existingFileName={props.audioFileName ?? ""} onFileUpload={uploadAudio} onFileDelete={deleteAudioHandler} />
                </div>
            )}
        </div>
    );
}; 
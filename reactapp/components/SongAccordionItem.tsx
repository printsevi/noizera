import React, { useEffect, useState } from 'react';
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
import { axiosPublic } from '@/libs/axios';

interface Props {
    id: string,
    key: string,
    isOpen: boolean,
    title?: string,
    audioFileName?: string;
    contentLength?: number;
    contentType?: string;
    songPublicId: string;
    toggleAccordion: () => void,
    onTitleUpdate: (newTitle: string) => Promise<boolean>;
    onAudioUpload: (file: File) => Promise<boolean>;
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

    const [audioSrc, setAudioSrc] = useState(props.audioFileName ? `${getURL()}api/songs/${props.songPublicId}/audio?audioType=original&contentLength=${props.contentLength}` : "");

    const { user } = useUser();

    useEffect(() => {
        if (props.songPublicId && props.audioFileName && props.contentLength && props.contentType) {
            setAudioSrc(`${getURL()}api/songs/${props.songPublicId}/audio?audioType=original&contentLength=${props.contentLength}`);
        } else {
            setAudioSrc("");
        }
    }, [props.songPublicId, props.audioFileName, props.contentLength, props.contentType]);

    const updateTitle = async (newTitle: string) => {
        const result = await props.onTitleUpdate(newTitle);
        if (!result) {
            setSongTitle(props.title ?? "");
        }
    }

    const deleteAudioHandler = async () => {
        const response = await props.onAudioDelete();
    }

    return (
        <div
            ref={setNodeRef}
            style={style}
            className="border rounded-md mb-3 ring-offset-background hover:outline-none hover:ring-2 hover:ring-ring hover:ring-offset-2"
        >
            <div
                className="w-full p-2 text-left transition duration-300 select-none"
                onClick={props.toggleAccordion}
            >
                <div className='flex flex-row items-center w-full justify-between'>
                    <div className='flex items-center p-2.5'
                        {...attributes}
                        {...listeners}>
                        <Grip size={20} className="flex-shrink-0" />
                    </div>
                    <div className='flex-grow text-center truncate px-2'>
                        <span className='truncate inline-block max-w-full align-middle'>
                            <span className='text-xs'>{user?.name ?? user?.username}</span> - <span className='text-sm font-medium'>{songTitle ? songTitle : 'Track ID'}</span>
                        </span>
                    </div>
                    <div className='flex items-center'>
                        <Button
                            variant='ghost'
                            className="rounded-full items-center mr-2"
                            size="icon"
                            onClick={(e) => {
                                e.stopPropagation();
                                props.onSongDelete();
                            }}
                        >
                            <Trash2 size={20} />
                        </Button>
                        <ChevronDown
                            size={20}
                            className={`transform transition-transform duration-300 ${props.isOpen ? 'rotate-180' : 'rotate-0'}`}
                        />
                    </div>
                </div>
            </div>
            <div className={`p-4 pt-1 ${props.isOpen ? '' : 'hidden'}`}>
                <Label className='block mb-1'>Song title</Label>
                <Input
                    value={songTitle}
                    onBlur={(value) => updateTitle(value.target.value)}
                    onChange={(value) => setSongTitle(value.target.value.slice(0, 100))}
                    placeholder='type your song title'
                    maxLength={100}
                />
                <Label className='block mb-1 mt-2.5'>Audio</Label>
                <AudioUploader
                    uploadTime='00/00/0000'
                    existingFileName={props.audioFileName ?? ""}
                    onFileUpload={props.onAudioUpload}
                    onFileDelete={deleteAudioHandler}
                    audioSrc={audioSrc}
                    contentType={props.contentType}
                />
            </div>
        </div>
    );
}; 
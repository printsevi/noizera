import React, { useRef, useState } from 'react';
import { Toast } from 'primereact/toast';
import { FileUpload, FileUploadHandlerEvent } from 'primereact/fileupload';
import { ProgressBar } from 'primereact/progressbar';
import { Button } from 'primereact/button';
import { Tooltip } from 'primereact/tooltip';
import { Tag } from 'primereact/tag';

interface Props {
    onUpload: (files: File[]) => Promise<boolean>;
}

export const FileInput = (props: Props) => {
    const onFileUpload = async (event: FileUploadHandlerEvent) => {
        console.log(event);
        const result = await props.onUpload(event.files);
    };

    return (
        <div className="card">
            <FileUpload 
                customUpload
                uploadHandler={onFileUpload}
                accept="audio/*" 
                maxFileSize={100000000} 
                emptyTemplate={<p className="m-0">Drag and drop files to upload.</p>} />
        </div>
    )
};

'use client';

import * as React from 'react';
import {
  MdPlayArrow,
  MdPause,
  MdSkipNext,
  MdSkipPrevious,
  MdVolumeUp,
  MdVolumeOff,
} from 'react-icons/md';
import Image from "next/image"
import { CgSpinner } from 'react-icons/cg';
import AudioProgressBar from './AudioProgressBar';
import IconButton from './IconButton';
import VolumeInput from './VolumeInput';
import useSong from '@/hooks/useSong';
import { useEffect, useRef, useState } from 'react';
import { formatDurationDisplay, getURL } from '@/libs/helpers';
import { Loader2, Pause, PauseCircleIcon, Play, PlayCircleIcon, Repeat, Shuffle, SkipBack, SkipForward, Volume2, Volume2Icon, VolumeIcon, VolumeX, VolumeXIcon } from 'lucide-react';
import { Slider } from './ui/slider';
import { Button } from './ui/button';

export default function AudioPlayer() {
  const { currentSong, next, prev, play, isPlaying, queue } = useSong();

  const audioRef = useRef<HTMLAudioElement | null>(null);

  const [isReady, setIsReady] = useState(false);
  const [duration, setDuration] = useState(0);
  const [currrentProgress, setCurrrentProgress] = useState(0);
  const [buffered, setBuffered] = useState(0);
  const [volume, setVolume] = useState(0.2);
  const [showVolumeSlider, setShowVolumeSlider] = useState(false)

  const durationDisplay = formatDurationDisplay(duration);
  const elapsedDisplay = formatDurationDisplay(currrentProgress);

  const handleNext = () => {
    changeAudioProgress(0);
    next();
  };

  const handlePrev = () => {
    changeAudioProgress(0);
    prev();
  };

  const handleEnded = () => {
    play(false);
    handleNext();
  };

  const togglePlayPause = () => {
    if (isPlaying) {
      play(false);
    } else {
      play(true);
    }
  };

  useEffect(() => {
    if (isPlaying) {
      const playPromise = audioRef.current?.play();
      if (playPromise !== undefined) {
        playPromise.then(_ => {
        })
          .catch(error => {
            play(false);
          });
      }
    } else {
      audioRef.current?.pause();
    }
  }, [isPlaying]);

  useEffect(() => {
    play(false);
    changeAudioProgress(0);
    if (!currentSong?.id) {
      return;
    }
    const timeout = setTimeout(() => {
      play(true);
    }, 500);

    return () => {
      clearTimeout(timeout);
    };
  }, [currentSong?.id]);

  const handleBufferProgress: React.ReactEventHandler<HTMLAudioElement> = (e) => {
    const audio = e.currentTarget;
    const dur = audio.duration;
    if (dur > 0) {
      for (let i = 0; i < audio.buffered.length; i++) {
        if (audio.buffered.start(audio.buffered.length - 1 - i) < audio.currentTime) {
          const bufferedLength = audio.buffered.end(
            audio.buffered.length - 1 - i
          );
          setBuffered(bufferedLength);
          break;
        }
      }
    }
  };

  const changeAudioProgress = (value: number) => {
    if (!audioRef.current) return;
    audioRef.current.currentTime = value;
    setCurrrentProgress(value);
  };

  const handleMuteUnmute = () => {
    if (!audioRef.current) return;

    if (audioRef.current.volume !== 0) {
      audioRef.current.volume = 0;
    } else {
      audioRef.current.volume = 0.2;
    }
  };

  const handleVolumeChange = (volumeValue: number) => {
    if (!audioRef.current) return;
    audioRef.current.volume = volumeValue;
    setVolume(volumeValue);
  };

  if (!queue.length) {
    return <></>;
  }

  return (
    <div className="
      sticky bottom-0 bg-background border-t p-2 sm:p-4
    ">
      {currentSong?.id && (
        <audio
          ref={audioRef}
          key={currentSong?.id}
          preload="metadata"
          onDurationChange={(e) => setDuration(e.currentTarget.duration)}
          onEnded={handleEnded}
          onCanPlay={(e) => {
            e.currentTarget.volume = volume;
            setIsReady(true);
          }}
          onTimeUpdate={(e) => {
            setCurrrentProgress(e.currentTarget.currentTime);
            handleBufferProgress(e);
          }}
          onProgress={handleBufferProgress}
          onVolumeChange={(e) => setVolume(e.currentTarget.volume)}
        >
          <source src={`${getURL()}api/songs/${currentSong.id}/audio?audioType=${currentSong.contentType}&contentLength=${currentSong.contentLength}`} type={currentSong.contentType} />
        </audio>
      )}
      <AudioProgressBar
        duration={duration}
        currentProgress={currrentProgress}
        buffered={buffered}
        onValueChange={changeAudioProgress}
      />

      <div className="flex justify-between"
        onMouseEnter={() => setShowVolumeSlider(true)}
        onMouseLeave={() => setShowVolumeSlider(false)}>
        <div className="flex flex-1 items-center gap-2 justify-self-center">
          <Button onClick={handlePrev} variant="ghost" size="icon" className='rounded-full hover:bg-primary hover:text-primary-foreground transition-colors'>
            <SkipBack className="h-6 w-6" />
          </Button>
          <Button disabled={!isReady} variant="ghost" onClick={togglePlayPause} size="icon" className='rounded-full hover:bg-primary hover:text-primary-foreground transition-colors'>
            {!isReady && currentSong ? (
              <Loader2 className="h-10 w-10 animate-spin" />
            ) : isPlaying ? (
              <Pause className="h-10 w-10" />
            ) : (
              <Play className="h-10 w-10" />
            )}

          </Button>
          <Button onClick={handleNext} variant="ghost" size="icon" className='rounded-full hover:bg-primary hover:text-primary-foreground transition-colors'>
            <SkipForward className="h-6 w-6" />
          </Button>
          <span className="text-xs hidden sm:block">
            {elapsedDisplay}&nbsp;/&nbsp;{durationDisplay}
          </span>
        </div>
        <div className="flex items-center justify-center flex-1">
          <Image
            src={currentSong?.coverPath ?? ""}
            alt="Cover"
            width={500}
            height={500}
            className="w-10 h-10 aspect-square object-cover rounded-md flex-col" />
          <div className="flex flex-col ml-1">
            <h3 className="font-medium text-sm truncate">{currentSong?.title ?? 'Track ID'}</h3>
            <p className="text-xs text-muted-foreground truncate">Artist Name</p>
          </div>
        </div>
        <div className="flex items-center justify-end space-x-2 flex-1">
          {showVolumeSlider && <Slider
            defaultValue={[0.2]}
            min={0}
            max={1}
            step={0.01}
            value={[volume]}
            onValueChange={(e) => handleVolumeChange(e[0])}
            className='max-w-28 hidden sm:inline-flex'
          />}
        </div>
        <div className="flex items-center justify-end space-x-2 flex-1">
          <Button onClick={handleMuteUnmute} variant="ghost" size="icon" className="hidden sm:inline-flex rounded-full hover:bg-primary hover:text-primary-foreground transition-colors">
            {volume === 0 ? (
              <VolumeX className="h-6 w-6" />
            ) : (
              <Volume2 className="h-6 w-6" />
            )}
          </Button>
          <Button variant="ghost" size="icon" className='rounded-full hover:bg-primary hover:text-primary-foreground transition-colors'>
            <Shuffle className="h-6 w-6" />
          </Button>
          <Button variant="ghost" size="icon" className='rounded-full hover:bg-primary hover:text-primary-foreground transition-colors'>
            <Repeat className="h-6 w-6" />
          </Button>
        </div>
      </div>
    </div>
  );
}

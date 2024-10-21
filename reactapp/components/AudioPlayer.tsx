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
import { CgSpinner } from 'react-icons/cg';
import AudioProgressBar from './AudioProgressBar';
import IconButton from './IconButton';
import VolumeInput from './VolumeInput';
import useSong from '@/hooks/useSong';
import { useEffect, useRef, useState } from 'react';
import { getURL } from '@/libs/helpers';
import { PauseCircleIcon, PlayCircleIcon, Volume2Icon, VolumeIcon, VolumeXIcon } from 'lucide-react';
import { Slider } from './ui/slider';

function formatDurationDisplay(duration: number) {
  const min = Math.floor(duration / 60);
  const sec = Math.floor(duration - min * 60);
  const formatted = [min, sec].map((n) => (n < 10 ? '0' + n : n)).join(':');
  return formatted;
}

export default function AudioPlayer() {
  const { currentSong, next, prev, play, isPlaying, queue } = useSong();

  const audioRef = useRef<HTMLAudioElement | null>(null);

  const [isReady, setIsReady] = useState(false);
  const [duration, setDuration] = useState(0);
  const [currrentProgress, setCurrrentProgress] = useState(0);
  const [buffered, setBuffered] = useState(0);
  const [volume, setVolume] = useState(0.2);

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
      bg-slate-900 
      p-3 
      sticky 
      bottom-0
      bg-black 
      w-full 
      py-2 
      h-[70px] 
      px-4
      z-10
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
          <source src={`${getURL()}api/songs/${currentSong.id}/flac-audio`} type="audio/flac" />
        </audio>
      )}
      <AudioProgressBar
        duration={duration}
        currentProgress={currrentProgress}
        buffered={buffered}
        onValueChange={changeAudioProgress}
      />

      <div className="flex justify-between">
        <div className="flex flex-1 items-center gap-4 justify-self-center">
          <IconButton
            onClick={handlePrev}
            aria-label="go to previous"
            intent="secondary"
          >
            <MdSkipPrevious size={24} />
          </IconButton>
          <IconButton
            disabled={!isReady}
            onClick={togglePlayPause}
            aria-label={isPlaying ? 'Pause' : 'Play'}
            size="lg"
          >
            {!isReady && currentSong ? (
              <CgSpinner size={24} className="animate-spin" />
            ) : isPlaying ? (
              <PauseCircleIcon size={30} />
            ) : (
              <PlayCircleIcon size={30} />
            )}
          </IconButton>
          <IconButton
            onClick={handleNext}
            aria-label="go to next"
            intent="secondary"
          >
            <MdSkipNext size={24} />
          </IconButton>
          <span className="text-xs">
            {elapsedDisplay} / {durationDisplay}
          </span>
        </div>
        <div className="flex-1 text-center mb-1">
          <p className="text-slate-300 font-bold">
            {currentSong?.title ?? 'Select a song'}
          </p>
          <p className="text-xs">Singer Name</p>
        </div>
        <div className="flex flex-1 gap-3">
          <Slider
            defaultValue={[0.2]}
            min={0}
            max={1}
            step={0.01}
            value={[volume]}
            onValueChange={(e) => handleVolumeChange(e[0])}
          />
          <IconButton
            intent="secondary"
            size="sm"
            onClick={handleMuteUnmute}
            aria-label={volume === 0 ? 'unmute' : 'mute'}
          >
            {volume === 0 ? (
              <VolumeXIcon size={20} />
            ) : (
              <Volume2Icon size={20} />
            )}
          </IconButton>
        </div>
      </div>
    </div>
  );
}

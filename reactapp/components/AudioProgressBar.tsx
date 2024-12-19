import React, { useEffect, useState } from 'react';
import * as SliderPrimitive from '@radix-ui/react-slider';

interface AudioProgressBarProps {
  duration: number;
  currentProgress: number;
  buffered: number;
  onValueChange: (value: number) => void;
}

export default function AudioProgressBar({ duration, currentProgress, buffered, onValueChange }: AudioProgressBarProps) {
  const [sliderValue, setSliderValue] = useState(currentProgress);

  useEffect(() => {
    setSliderValue(currentProgress);
  }, [currentProgress]);

  const progressBarWidth = isNaN(currentProgress / duration) ? 0 : currentProgress / duration;
  const bufferedWidth = isNaN(buffered / duration) ? 0 : buffered / duration;

  return (
    <SliderPrimitive.Root
      className="relative flex w-full touch-none select-none items-center group h-1"
      value={[sliderValue]}
      onValueChange={(newValue) => {
        setSliderValue(newValue[0]);
        onValueChange(newValue[0]);
      }}
      max={duration}
      step={0.01}
    >
      <SliderPrimitive.Track className="relative h-full w-full grow overflow-hidden bg-secondary">
        <SliderPrimitive.Range className="absolute h-full bg-primary" />
      </SliderPrimitive.Track>
      <SliderPrimitive.Thumb
        className="block h-4 w-4 rounded-full border-2 border-primary bg-background ring-offset-background transition-all duration-150 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 hover:h-6 hover:w-6 cursor-pointer"
      />
    </SliderPrimitive.Root>
  );
}


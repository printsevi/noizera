import { Combobox, ComboboxButton, ComboboxInput, ComboboxOption, ComboboxOptions, Transition } from '@headlessui/react'
import { FaChevronDown } from "react-icons/fa";
import { RiUserFollowLine } from "react-icons/ri";
import clsx from 'clsx'
import { useEffect, useState } from 'react'
import useSWR from 'swr';
import { debounce } from 'lodash';
import { v4 as uuidv4 } from 'uuid';

interface Props {
    initialSelectedItems: ComboboxItemProps[];
    label: string;
    id: string;
    getItems: (text: string) => Promise<ComboboxItemProps[]>;
    apiUrl: string;
    onItemAdd: (item: ComboboxItemProps) => Promise<string | undefined>;
    onItemDelete: (creditId?: string) => Promise<boolean>;
    showCreateNewOption?: boolean
}

export interface ComboboxItemProps {
    key?: string,
    value?: string;
}

interface IdItem {
  key: string;
  id: string;
}

const compareItems = (a?: ComboboxItemProps, b?: ComboboxItemProps): boolean =>
  a?.value?.toLowerCase() === b?.value?.toLowerCase();
  
export const ComboboxField = (props: Props) => {
  const [query, setQuery] = useState('');
  const [selected, setSelected] = useState(props.initialSelectedItems);
  const [selectedDb, setSelectedDb] = useState(props.initialSelectedItems);
  const [selectedIdItems, setSelectedIdItems] = useState<IdItem[]>([]);
  const [updateInProgress, setUpdateInProgress] = useState(false);
  const {data : filteredItems, error, isLoading} = useSWR(`${props.apiUrl}?text=${query}`, () => props.getItems(query), {
    keepPreviousData: true,
    revalidateIfStale: false,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });

  const onQueryChange = (event: any) => {
    setQuery(event.target.value as string);
  }

  const debouncedOnQueryChange = debounce(onQueryChange, 500);

  const removeItem = (item: ComboboxItemProps) => {
    setSelected(selectedItems => selectedItems.filter(x => x.key !== item.key));
  }

  useEffect(() => {
    setSelectedDb(props.initialSelectedItems);
    setSelected(props.initialSelectedItems);
  }, [props.initialSelectedItems]);

  useEffect(() => {
    async function updateItemsDb() {
      if (selected.length > selectedDb.length) {
        const itemForAdd = selected.find(x => !selectedDb.map(x => x.key).includes(x.key));
        const itemId = await props.onItemAdd(itemForAdd!);
        if (itemId) {
          setSelectedIdItems(items => [...items, {key: itemForAdd?.key ?? "", id: itemId} ]);
          setSelectedDb(selected);
        } else {
          setSelected(selectedDb);
        }
      } else if (selected.length < selectedDb.length) {
        const deleteItemKey = selectedDb.find(x => !selected.map(x => x.key).includes(x.key))?.key;
        const isDeleted = await props.onItemDelete(selectedIdItems.find(x => x.key === deleteItemKey)?.id);
        if (isDeleted) {
          setSelectedIdItems(selectedItems => selectedItems.filter(x => x.key !== deleteItemKey));
          setSelectedDb(selected);
        } else {
          setSelected(selectedDb);
        }
      }
    }

    setUpdateInProgress(true);
    updateItemsDb();
    setUpdateInProgress(false);
  }, [selected]);

  return (
    <div className="pt-3 w-52">
      <div className="flex justify-between">
        <label htmlFor={props.id}>
          {props.label}
        </label>
      </div>
      <Combobox 
        multiple 
        disabled={updateInProgress}
        value={selected} 
        onChange={setSelected}
        by={compareItems}
      >
        {selected.length > 0 && (
          <div>
            {selected.map((item) => (
              <span key={`${item.key}-selected-item`} className="inline-flex items-center gap-x-1.5 py-1.5 ps-3 pe-2 rounded-full text-sm bg-neutral-700 border border-transparent">
                {item.value}
                <button 
                  onClick={() => removeItem(item)}
                  type="button" 
                  className="flex-shrink-0 size-4 inline-flex items-center justify-center rounded-full hover:bg-blue-200 focus:outline-none focus:bg-blue-200 focus:text-blue-500 dark:hover:bg-blue-900"
                >
                  <span className="sr-only">Remove badge</span>
                  <svg className="flex-shrink-0 size-3" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <path d="M18 6 6 18"></path>
                    <path d="m6 6 12 12"></path>
                  </svg>
                </button>
            </span>
            ))}
          </div>
        )}
        <div className="relative">
          <ComboboxInput
            className={clsx(
              'w-full rounded-lg border-none bg-white/5 py-1.5 pr-8 pl-3 text-sm/6 text-white',
              'focus:outline-none data-[focus]:outline-2 data-[focus]:-outline-offset-2 data-[focus]:outline-white/25'
            )}
            aria-label="Assignees"
            onChange={debouncedOnQueryChange}
            placeholder='Start typing...'
          />
          <ComboboxButton className="group absolute inset-y-0 right-0 px-2.5">
            <FaChevronDown />
          </ComboboxButton>
        </div>
        <Transition
          leave="transition ease-in duration-100"
          leaveFrom="opacity-100"
          leaveTo="opacity-0"
          afterLeave={() => setQuery('')}
        >
          <ComboboxOptions
            anchor="bottom"
            className="w-[var(--input-width)] rounded-xl border border-white/5 bg-neutral-900  p-1 [--anchor-gap:var(--spacing-1)] empty:hidden"
          >
            {props.showCreateNewOption && !isLoading && !filteredItems?.filter(x => x.value === query).length && !selected?.filter(x => x.value === query).length && query.length && (
              <ComboboxOption 
                value={{ id: uuidv4(), name: query, toCreate: true }} 
                className="group flex cursor-default items-center gap-2 rounded-lg py-1.5 px-3 select-none data-[focus]:bg-white/10"
              >
                Create <span className="font-bold">&quot;{query}&quot;</span>
              </ComboboxOption>
            )}
            {filteredItems?.map((filteredItem) => (
              <ComboboxOption
                key={filteredItem.key}
                value={filteredItem}
                className="group flex cursor-default items-center gap-2 rounded-lg py-1.5 px-3 select-none data-[focus]:bg-white/10"
              >
                <RiUserFollowLine />
                <div className="text-sm/6 text-white">{filteredItem.value}</div>
              </ComboboxOption>
            ))}
          </ComboboxOptions>
        </Transition>
      </Combobox>
    </div>
  )
}
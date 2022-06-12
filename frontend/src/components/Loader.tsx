import React from 'react';
import loader from '../assets/loader.gif';

interface ILoaderProps {
  message?: string;
}

const Loader: React.FC<ILoaderProps> = ({ message }) => {
  return (
    <div className="loader_wrapper">
      <img src={loader} />
      <span>{message ?? 'Загрузка...'}</span>
    </div>
  );
};

export default Loader;

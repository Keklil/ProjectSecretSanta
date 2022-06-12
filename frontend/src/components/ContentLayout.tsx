import React from 'react';
import { useNavigate } from 'react-router-dom';
import { ReactComponent as Back } from '../assets/back.svg';

interface IContentLayoutProps {
  user: IUser;
  endPoint: string;
  handleExitGame(eventId?: string): Promise<boolean>;
}

const ContentLayout: React.FC<IContentLayoutProps> = ({ children, user, handleExitGame, endPoint }) => {
  const nav = useNavigate();
  const withExitBtn = user.role === 'user' && endPoint === 'game' && !user.activeEvent?.reshuffle;
  // console.log(user.role);
  // console.log(endPoint);
  // console.log(user.activeEvent);
  return (
    <div className="main_content__layout">
      <div className="main_content__layout_header">
        <Back className="default_back_btn" onClick={() => nav(-1)} />
        {/* <div></div> */}
        {withExitBtn && (
          <>
            <div></div>
            <button
              onClick={() => {
                console.log(user.activeEvent);
                handleExitGame(user.activeEvent?.id).then((ok) => {
                  if (ok) nav('../');
                });
              }}
              className="default__btn"
            >
              Покинуть
            </button>
          </>
        )}
      </div>
      <div className="main_content_layout_content">{children}</div>
    </div>
  );
};

export default ContentLayout;

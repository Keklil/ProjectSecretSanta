import React, { useEffect, useState } from 'react';
import './style.css';
import playerIcon from '../../../assets/playerIcon.png';
import useDebounce from '../../../hooks/useDebounce';

interface IFindMemberProps extends IDefaultProps {
  gameData: IExistEvent;
}

const FindMember: React.FC<IFindMemberProps> = ({ serverController, user, gameData }) => {
  const [searchInput, setSearchInput] = useState('');
  const searchValue = useDebounce(searchInput, 500);
  const [members, setMembers] = useState<any[]>([]);

  const searchFilter = (value: any, index: number, array: any[]) => {
    const { surname, name, patronymic } = value.memberView;
    const text = `${surname} ${name} ${patronymic}`;
    console.log(text);
    console.log(value);
    console.log(searchValue);
    return text.includes(searchValue);
  };

  useEffect(() => {
    setMembers(gameData.memberView?.filter(searchFilter));
  }, [searchValue]);

  return (
    <>
      <section className="container">
        <div className="info_window">
          <p className="info_numbers">{new Date(String(gameData?.endRegistration)).toLocaleDateString()}</p>
          <span className="info_description">Дата жеребьёвки</span>
        </div>
        <div className="info_window">
          <p className="info_numbers">{new Date(String(gameData?.endEvent)).toLocaleDateString()}</p>
          <span className="info_description">Отправить подарки до</span>
        </div>
        <div className="info_window">
          <p className="info_numbers">{gameData.membersCount}</p>
          <span className="info_description">Кол-во участников</span>
        </div>
      </section>
      <section className="container_fullname">
        <input className="input_fullname" type="text" placeholder="введите ФИО участника" onChange={(e) => setSearchInput(e.target.value)} />
        {/* <button className="button_find"></button> */}
      </section>
      <section className="container_members">
        {members.map((item, index) => {
          const { memberView } = item;
          const name = `${memberView.surname} ${memberView.name} ${memberView.patronymic}`;
          console.log(memberView);
          return (
            <div key={index} className="default_player_card">
              <div className="default_player_card__image">
                <img src={playerIcon} />
              </div>
              <div className="default_player_card__name">{name}</div>
            </div>
          );
        })}
      </section>
    </>
  );
};

export default FindMember;

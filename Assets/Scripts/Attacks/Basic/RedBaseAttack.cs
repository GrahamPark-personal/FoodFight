using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Attacks.Basic
{
    class RedBaseAttack : Attack
    {

        Cell mCell;

        public override void Init()
        {
            CreateID();

            SetDamage(16);
            SetRange(7);
            SetRadius(1);
            SetAOE(1);

            SetStartPos(GameManager.sInstance.mSelectedCell);
            GameManager.sInstance.mAttackShape = AttackShape.Area;

            GameManager.sInstance.mCurrentRange = GetRange();

            GameManager.sInstance.mPreviewShape = HoverShape.SingleSpot;

        }

        public override void Exit()
        {

        }

        public override void Execute(IntVector2 pos)
        {

            Character temp = GameManager.sInstance.mCharacterObj;
            //Character temp = GameManager.sInstance.mCurrGrid.rows[GetStartPos().y].cols[GetStartPos().x].mCharacterObj;

            IntVector2 destination = pos;

            mCell = GameManager.sInstance.mCurrGrid.rows[destination.y].cols[destination.x]; // set mCell to currently selected cell

            //Add an ailment that does the following
            // first check if stunned/taunted
            // if not disabled: teleport back to cast location

            MeshRenderer mMeshRenderer = temp.GetComponent<MeshRenderer>();

            // Red guy submerges below the ground (for now disable mesh renderer)

            temp.returnLocation = GameManager.sInstance.mCurrGrid.rows[GetStartPos().y].cols[GetStartPos().x];
            mMeshRenderer.enabled = false;

            GameManager.sInstance.mCurrGrid.rows[temp.mCellPos.y].cols[temp.mCellPos.x].mTypeOnCell = TypeOnCell.nothing;
            GameManager.sInstance.mCurrGrid.rows[temp.mCellPos.y].cols[temp.mCellPos.x].mCannotMoveHere = false;
            GameManager.sInstance.mCurrGrid.rows[temp.mCellPos.y].cols[temp.mCellPos.x].mCharacterObj = null;

            IntVector2 targetCell = temp.mCellPos;

            GameManager.sInstance.IsMovableBlock(targetCell);


            do
            {
                targetCell = destination;
                targetCell.x -= 1;
                if (GameManager.sInstance.IsMovableBlock(targetCell))
                {
                    break;
                }

                targetCell = destination;
                targetCell.x += 1;
                if (GameManager.sInstance.IsMovableBlock(targetCell))
                {
                    break;
                }

                targetCell = destination;
                targetCell.y += 1;
                if (GameManager.sInstance.IsMovableBlock(targetCell))
                {
                    break;
                }

                targetCell = destination;
                targetCell.y -= 1;
                if (GameManager.sInstance.IsMovableBlock(targetCell))
                {
                    break;
                }

            }
            while (false);


            temp.mPosition.position = GameManager.sInstance.mCurrGrid.rows[targetCell.y].cols[targetCell.x].transform.position + new Vector3(0, 1, 0);
            temp.mCellPos = targetCell;
            //targetcell is now the end position

            GameManager.sInstance.mCurrGrid.rows[temp.mCellPos.y].cols[temp.mCellPos.x].mTypeOnCell = TypeOnCell.character;
            GameManager.sInstance.mCurrGrid.rows[temp.mCellPos.y].cols[temp.mCellPos.x].mCannotMoveHere = false;
            GameManager.sInstance.mCurrGrid.rows[temp.mCellPos.y].cols[temp.mCellPos.x].mCharacterObj = temp;

            // Move Character to Target Location
            // Applies Damage

            if (GameManager.sInstance.mCurrGrid.rows[destination.y].cols[destination.x].mTypeOnCell == TypeOnCell.enemy)
            {
                mCell.mEnemyObj.Damage(GameManager.sInstance.mCharacterObj, GetDamage());
            }



            mMeshRenderer.enabled = true;
            // Resurfaces at location  (enable mesh renderer)
            temp.needsToReturn = true;
            //Mark character to be teleported at the beginning of next turn


        }
    }
}
